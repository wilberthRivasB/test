using PlantillaMVC.Domain.Models;
using PlantillaMVC.Domain.Services;
using PlantillaMVC.Integrations;
using PlantillaMVC.Integrations.Hubspot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace PlantillaMVC.Jobs.Jobs
{
    public class TicketsSyncJob
    {
        static bool executing = false;
        static readonly Object thisLock = new Object();

        public static void SyncTickets()
        {
            bool ProcessEnabled = true;
            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["Jobs.EnabledJobs"], out ProcessEnabled);
            int definitionId = 0;
            if(!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["DefinitionId"], out definitionId))
            {
                throw new Exception("DefinitionId no existe en el web config");
            }
            string pipelineId = System.Configuration.ConfigurationManager.AppSettings["PipelineId"];
            string pipelineStageId = System.Configuration.ConfigurationManager.AppSettings["PipelineStageId"];
            IHubspotService apiService = new HubspotService();
            Trace.TraceInformation(string.Format("[TicketsSyncJob.SyncTickets] Executing at {0}", DateTime.Now));
            IDBService dbService = new DBService();
            DBProceso procesoInfo = dbService.GetProcessInfo("SINCRONIZACION_TICKETS");

            StringBuilder strResultado = new StringBuilder("Iniciando proceso...");
            if (Monitor.TryEnter(thisLock))
            {
                try
                {
                    if (!executing && ProcessEnabled)
                    {
                        executing = true;
                        //SI ESTA HABILITADO Y NO SE ESTA EJECUTANDO
                        if (!procesoInfo.EstatusEjecucion && procesoInfo.EstatusProceso)
                        {
                            procesoInfo.EstatusEjecucion = true;
                            procesoInfo.UltimaEjecucion = DateTime.Now;
                            procesoInfo.Resultado = strResultado.ToString();
                            dbService.ActualizarEstatusProceso(procesoInfo);

                            DBProcesoEjecucion procesoDetalle = new DBProcesoEjecucion()
                            {
                                ProcesoId = procesoInfo.ProcesoId,
                                Estatus = true,
                                Resultado = "Procesando..."
                            };
                            int ProcesoDetalleId = dbService.CreateProcesoEjecucion(procesoDetalle);
                            #region SINCRONIZACION DE COMPANIAS EN MEMORIA
                            Dictionary<string, long> CompanyDictionary = new Dictionary<string, long>();
                            long offset = 0;
                            bool hasMoreCompanies = true;
                            int totalCompanies = 0;

                            strResultado.Append(" * Paso 1 ");
                            while (hasMoreCompanies)
                            {

                                strResultado.Append(" * Paso 1.1 ");
                                CompaniesHubSpotResult companiesHubSpotResult = apiService.GetAllCompanies(250, offset);
                                Trace.TraceInformation(string.Format("HasMore: {0} Offset: {1}", companiesHubSpotResult.HasMore, companiesHubSpotResult.Offset));
                                hasMoreCompanies = companiesHubSpotResult.HasMore;
                                offset = companiesHubSpotResult.Offset;

                                strResultado.Append(" * Paso 1.2 ");
                                totalCompanies += companiesHubSpotResult.Companies.Count();

                                strResultado.Append(" * Paso 1.3 ");
                                foreach (Company company in companiesHubSpotResult.Companies)
                                {
                                    //TODO: Cambiar por RFC
                                    if (company.Properties.RFC != null && !string.IsNullOrEmpty(company.Properties.RFC.Value))
                                    {
                                        string rfcCompany = company.Properties.RFC.Value.Trim().ToUpper();
                                        if (!CompanyDictionary.ContainsKey(rfcCompany)) CompanyDictionary.Add(rfcCompany, company.CompanyId);
                                    }
                                }
                            }
                            Trace.TraceInformation(string.Format("Total Companies: {0}", totalCompanies));
                            Trace.TraceInformation(string.Format("Total Companies in Dic: {0}", CompanyDictionary.Count()));
                            #endregion
                            strResultado.Append(" * Paso 2 ");
                            dbService.UpdateTicketsToProcess();
                            strResultado.Append(" * Paso 3 ");
                            IEnumerable<DBTicketModel> tickets = dbService.GetTickets();
                            strResultado.Append(" * Paso 4 ");
                            int ticketsSyncronized = 0;
                            foreach (DBTicketModel ticket in tickets)
                            {
                                long companyId = 0;
                                strResultado.Append(" * Paso 4.1 ");
                                string rfc = ticket.RFC.Trim().ToUpper();
                                if (!CompanyDictionary.TryGetValue(rfc, out companyId))
                                {
                                    strResultado.Append(String.Format("No existe el rfc {0} en el hubspot", rfc));
                                    continue;
                                }
                                strResultado.Append(" * Paso 4.2 ");
                                CompanyTicketHubspotSave ticketHubspotSave = new CompanyTicketHubspotSave
                                {
                                    CompanyId = companyId,
                                    Content = ticket.Descripcion,
                                    Monto = ticket.Monto,
                                    NumeroOperacion = ticket.NumeroOperacion,
                                    Subject = ticket.TipoActividad,
                                    DefinitionId = definitionId,
                                    PipelineId = pipelineId,
                                    PipelineStageId = pipelineStageId
                                };

                                strResultado.Append(" * Paso 4.3 ");
                                CompanyTicketCreateHubspotResponse apiResponse = apiService.CreateTicketToCompany(ticketHubspotSave);
                                strResultado.Append(" * Paso 4.4 ");
                                if(apiResponse.IsCreated)
                                {
                                    strResultado.Append(" * Paso 4.5 ");
                                    ticket.TicketId = apiResponse.TicketId;
                                    dbService.UpdateSyncTicket(ticket);
                                    ticketsSyncronized++;
                                }
                            }
                            Trace.TraceInformation(string.Format("[TicketsSyncJob.SyncTickets] Finishing at {0}", DateTime.Now));
                            int total = tickets.Count();
                            procesoDetalle.FechaFin = DateTime.Now;
                            procesoDetalle.Estatus = false;
                            procesoDetalle.Resultado = string.Format("Se sincronizaron {0} de {1} tickets", ticketsSyncronized, total);
                            strResultado.Append(string.Format("|Se sincronizaron {0} de {1} tickets", ticketsSyncronized, total));
                            dbService.ActualizarProcesoEjecucion(procesoDetalle);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    Trace.TraceInformation(exception.Message);

                    strResultado.Append("|" + exception.Message);

                    if (exception.Source != null)
                    {
                        strResultado.Append("|" + exception.Source);
                    }
                    if (exception.StackTrace != null)
                    {
                        strResultado.Append("|" + exception.StackTrace);
                    }

                    try
                    {
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(exception, true);
                        strResultado.Append("|" + String.Format("<p>Error Detail Message :{0}  => Error In :{1}  => Line Number :{2} => Error Method:{3}</p>",
                                  HttpUtility.HtmlEncode(exception.Message),
                                  trace.GetFrame(0).GetFileName(),
                                  trace.GetFrame(0).GetFileLineNumber(),
                                  trace.GetFrame(0).GetMethod().Name));
                    }
                    catch (Exception ex) { }
                }
                finally
                {
                    procesoInfo.EstatusEjecucion = false;
                    procesoInfo.UltimaEjecucion = DateTime.Now;
                    procesoInfo.Resultado = strResultado.ToString();
                    dbService.ActualizarEstatusProceso(procesoInfo);

                    executing = false;
                    Monitor.Exit(thisLock);
                }
            }
            Trace.TraceInformation(string.Format("[TicketsSyncJob.SyncTickets] Finishing at {0}", DateTime.Now));
        }
    }
}