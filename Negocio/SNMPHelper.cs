using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using System.Net;
using Dominio;
using System.Security.Cryptography;
using Negocio;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;


public class SNMPHelper
{
    private string community = "public";
    private int timeout = 1000;
    private Dictionary<string, Oids> oidsDictionary = new Dictionary<string, Oids>();
    private List<DatosImpresora> printersList = new List<DatosImpresora>();
    ImpresorasNegocio impresorasNegocio = new ImpresorasNegocio();
    OidsNegocio oidsNegocio = new OidsNegocio();

    public void LoadPrintersFromCsv(string filePath)
    {
        printersList.Clear();
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines.Skip(1)) // Saltar encabezado
        {
            var values = line.Split(';');
            if (values.Length < 3) continue;
            printersList.Add(new DatosImpresora
            {
                Model = values[0],
                ip = values[1],
                ubi = values[2]
            });
        }
    }

    public void LoadOidsFromCsv(string filePath)
    {
        oidsDictionary.Clear();
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines.Skip(1)) // Saltar encabezado
        {
            var values = line.Split(';');
            if (values.Length < 13) continue;
            oidsDictionary[values[0]] = new Oids
            {
                oidMac = values[1],
                oidModel = values[2],
                oidSerial = values[3],
                oidPageCount = values[4],
                oidBlackTonerFULL = values[5],
                oidCyanTonerFULL = values[6],
                oidMagentaTonerFULL = values[7],
                oidYellowTonerFULL = values[8],
                oidBlackToner = values[9],
                oidCyanToner = values[10],
                oidMagentaToner = values[11],
                oidYellowToner = values[12],
                oidWasteContainer = values[13],
                oidUnitImageFULL = values[14],
                oidUnitImage = values[15]
            }; 
        }
    }

    public List<DatosImpresora> GetPrintersInfo()
    {
        foreach (var printer in printersList)
        {
            if (oidsDictionary.ContainsKey(printer.Model))
            {
                var oids = oidsDictionary[printer.Model];
                var oidList = new List<string>
            {
                oids.oidMac, oids.oidModel, oids.oidSerial, oids.oidPageCount,
                oids.oidBlackToner, oids.oidBlackTonerFULL
            };

                // Agregar solo los OIDs de toner de color si no están vacíos
                if (!string.IsNullOrWhiteSpace(oids.oidCyanToner) && !string.IsNullOrWhiteSpace(oids.oidCyanTonerFULL))
                {
                    oidList.Add(oids.oidCyanToner);
                    oidList.Add(oids.oidCyanTonerFULL);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidMagentaToner) && !string.IsNullOrWhiteSpace(oids.oidMagentaTonerFULL))
                {
                    oidList.Add(oids.oidMagentaToner);
                    oidList.Add(oids.oidMagentaTonerFULL);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidYellowToner) && !string.IsNullOrWhiteSpace(oids.oidYellowTonerFULL))
                {
                    oidList.Add(oids.oidYellowToner);
                    oidList.Add(oids.oidYellowTonerFULL);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidWasteContainer))
                {
                    oidList.Add(oids.oidWasteContainer);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidUnitImage) && !string.IsNullOrWhiteSpace(oids.oidUnitImageFULL))
                {
                    oidList.Add(oids.oidUnitImage);
                    oidList.Add(oids.oidUnitImageFULL);
                }
                var responses = GetSNMPValues(printer.ip, oidList);

                printer.mac = responses.ContainsKey(oids.oidMac) ? responses[oids.oidMac] : "N/A";
                printer.serial = responses.ContainsKey(oids.oidSerial) ? responses[oids.oidSerial] : "N/A";
                printer.pageCount = ParseInt(responses, oids.oidPageCount);

                printer.blackToner = CalcularPorcentajeToner(responses, oids.oidBlackToner, oids.oidBlackTonerFULL);

                if (!string.IsNullOrWhiteSpace(oids.oidCyanToner) && !string.IsNullOrWhiteSpace(oids.oidCyanTonerFULL))
                    printer.cyanToner = CalcularPorcentajeToner(responses, oids.oidCyanToner, oids.oidCyanTonerFULL);
                else
                    printer.cyanToner = -1; // Marcador para ocultar en frontend

                if (!string.IsNullOrWhiteSpace(oids.oidMagentaToner) && !string.IsNullOrWhiteSpace(oids.oidMagentaTonerFULL))
                    printer.magentaToner = CalcularPorcentajeToner(responses, oids.oidMagentaToner, oids.oidMagentaTonerFULL);
                else
                    printer.magentaToner = -1;

                if (!string.IsNullOrWhiteSpace(oids.oidYellowToner) && !string.IsNullOrWhiteSpace(oids.oidYellowTonerFULL))
                    printer.yellowToner = CalcularPorcentajeToner(responses, oids.oidYellowToner, oids.oidYellowTonerFULL);
                else
                    printer.yellowToner = -1;

                if (!string.IsNullOrWhiteSpace(oids.oidWasteContainer))
                    printer.wasteContainer = ParseInt(responses, oids.oidWasteContainer);
                else
                    printer.wasteContainer = -1;

                if (!string.IsNullOrWhiteSpace(oids.oidUnitImage) && !string.IsNullOrWhiteSpace(oids.oidUnitImageFULL))
                    printer.unitImage = CalcularPorcentajeToner(responses, oids.oidUnitImage, oids.oidUnitImageFULL);
                else
                    printer.unitImage = -1;
            }
        }
        return printersList;
    }

    public List<DatosImpresora> GetPrintersInfoFromMySQL()
    {
        List<DatosImpresora> printersList = impresorasNegocio.ListarImpresoras();
        Dictionary<string, Oids> oidsDictionary = oidsNegocio.ListarOids();

        foreach (var printer in printersList)
        {
            // Se asume que la propiedad Model de DatosImpresora es la clave que coincide con la tabla modelos
            bool isOnline = PingPrinter(printer.ip);
            printer.status = isOnline ? "Online" : "Offline";
            if (oidsDictionary.ContainsKey(printer.Model))
            {
                var oids = oidsDictionary[printer.Model];
                var oidList = new List<string>
                {
                    oids.oidMac,
                    oids.oidModel,
                    oids.oidSerial,
                    oids.oidPageCount,
                    oids.oidBlackToner,
                    oids.oidBlackTonerFULL
                };

                // Agregar los OIDs de toner de color si existen
                if (!string.IsNullOrWhiteSpace(oids.oidCyanToner) && !string.IsNullOrWhiteSpace(oids.oidCyanTonerFULL))
                {
                    oidList.Add(oids.oidCyanToner);
                    oidList.Add(oids.oidCyanTonerFULL);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidMagentaToner) && !string.IsNullOrWhiteSpace(oids.oidMagentaTonerFULL))
                {
                    oidList.Add(oids.oidMagentaToner);
                    oidList.Add(oids.oidMagentaTonerFULL);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidYellowToner) && !string.IsNullOrWhiteSpace(oids.oidYellowTonerFULL))
                {
                    oidList.Add(oids.oidYellowToner);
                    oidList.Add(oids.oidYellowTonerFULL);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidWasteContainer))
                {
                    oidList.Add(oids.oidWasteContainer);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidUnitImage) && !string.IsNullOrWhiteSpace(oids.oidUnitImageFULL))
                {
                    oidList.Add(oids.oidUnitImage);
                    oidList.Add(oids.oidUnitImageFULL);
                }

                if (isOnline)
                {
                    var responses = GetSNMPValues(printer.ip, oidList);

                    printer.mac = responses.ContainsKey(oids.oidMac) ? responses[oids.oidMac] : "N/A";
                    printer.serial = responses.ContainsKey(oids.oidSerial) ? responses[oids.oidSerial] : "N/A";
                    printer.pageCount = ParseInt(responses, oids.oidPageCount);
                    printer.blackToner = CalcularPorcentajeToner(responses, oids.oidBlackToner, oids.oidBlackTonerFULL);

                    if (!string.IsNullOrWhiteSpace(oids.oidCyanToner) && !string.IsNullOrWhiteSpace(oids.oidCyanTonerFULL))
                        printer.cyanToner = CalcularPorcentajeToner(responses, oids.oidCyanToner, oids.oidCyanTonerFULL);
                    else
                        printer.cyanToner = -1;

                    if (!string.IsNullOrWhiteSpace(oids.oidMagentaToner) && !string.IsNullOrWhiteSpace(oids.oidMagentaTonerFULL))
                        printer.magentaToner = CalcularPorcentajeToner(responses, oids.oidMagentaToner, oids.oidMagentaTonerFULL);
                    else
                        printer.magentaToner = -1;

                    if (!string.IsNullOrWhiteSpace(oids.oidYellowToner) && !string.IsNullOrWhiteSpace(oids.oidYellowTonerFULL))
                        printer.yellowToner = CalcularPorcentajeToner(responses, oids.oidYellowToner, oids.oidYellowTonerFULL);
                    else
                        printer.yellowToner = -1;

                    if (!string.IsNullOrWhiteSpace(oids.oidWasteContainer))
                        printer.wasteContainer = ParseInt(responses, oids.oidWasteContainer);
                    else
                        printer.wasteContainer = -1;

                    if (!string.IsNullOrWhiteSpace(oids.oidUnitImage) && !string.IsNullOrWhiteSpace(oids.oidUnitImageFULL))
                        printer.unitImage = CalcularPorcentajeToner(responses, oids.oidUnitImage, oids.oidUnitImageFULL);
                    else
                        printer.unitImage = -1;
                }
                else
                {
                  
                    printer.mac = "N/A";
                    printer.serial = "N/A";
                    
                }
            }
        }

        return printersList;
    }

    public List<DatosImpresora> GetPrintersInfoFromMySQL(int ubicacionID)
    {
        List<DatosImpresora> printersList = impresorasNegocio.ListarImpresorasPorUbicacion(ubicacionID);
        Dictionary<string, Oids> oidsDictionary = oidsNegocio.ListarOids();

        foreach (var printer in printersList)
        {
            bool isOnline = PingPrinter(printer.ip);
            printer.status = isOnline ? "Online" : "Offline";
            // Se asume que la propiedad Model de DatosImpresora es la clave que coincide con la tabla modelos
            if (oidsDictionary.ContainsKey(printer.Model))
            {
                var oids = oidsDictionary[printer.Model];
                var oidList = new List<string>
                {
                    oids.oidMac,
                    oids.oidModel,
                    oids.oidSerial,
                    oids.oidPageCount,
                    oids.oidBlackToner,
                    oids.oidBlackTonerFULL
                };

                // Agregar los OIDs de toner de color si existen
                if (!string.IsNullOrWhiteSpace(oids.oidCyanToner) && !string.IsNullOrWhiteSpace(oids.oidCyanTonerFULL))
                {
                    oidList.Add(oids.oidCyanToner);
                    oidList.Add(oids.oidCyanTonerFULL);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidMagentaToner) && !string.IsNullOrWhiteSpace(oids.oidMagentaTonerFULL))
                {
                    oidList.Add(oids.oidMagentaToner);
                    oidList.Add(oids.oidMagentaTonerFULL);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidYellowToner) && !string.IsNullOrWhiteSpace(oids.oidYellowTonerFULL))
                {
                    oidList.Add(oids.oidYellowToner);
                    oidList.Add(oids.oidYellowTonerFULL);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidWasteContainer))
                {
                    oidList.Add(oids.oidWasteContainer);
                }
                if (!string.IsNullOrWhiteSpace(oids.oidUnitImage) && !string.IsNullOrWhiteSpace(oids.oidUnitImageFULL))
                {
                    oidList.Add(oids.oidUnitImage);
                    oidList.Add(oids.oidUnitImageFULL);
                }

                if (isOnline)
                {
                    // Ejecutar consulta SNMP para el dispositivo con ip de la impresora
                    var responses = GetSNMPValues(printer.ip, oidList);

                    printer.mac = responses.ContainsKey(oids.oidMac) ? responses[oids.oidMac] : "N/A";
                    printer.serial = responses.ContainsKey(oids.oidSerial) ? responses[oids.oidSerial] : "N/A";
                    printer.pageCount = ParseInt(responses, oids.oidPageCount);
                    printer.blackToner = CalcularPorcentajeToner(responses, oids.oidBlackToner, oids.oidBlackTonerFULL);

                    if (!string.IsNullOrWhiteSpace(oids.oidCyanToner) && !string.IsNullOrWhiteSpace(oids.oidCyanTonerFULL))
                        printer.cyanToner = CalcularPorcentajeToner(responses, oids.oidCyanToner, oids.oidCyanTonerFULL);
                    else
                        printer.cyanToner = -1;

                    if (!string.IsNullOrWhiteSpace(oids.oidMagentaToner) && !string.IsNullOrWhiteSpace(oids.oidMagentaTonerFULL))
                        printer.magentaToner = CalcularPorcentajeToner(responses, oids.oidMagentaToner, oids.oidMagentaTonerFULL);
                    else
                        printer.magentaToner = -1;

                    if (!string.IsNullOrWhiteSpace(oids.oidYellowToner) && !string.IsNullOrWhiteSpace(oids.oidYellowTonerFULL))
                        printer.yellowToner = CalcularPorcentajeToner(responses, oids.oidYellowToner, oids.oidYellowTonerFULL);
                    else
                        printer.yellowToner = -1;

                    if (!string.IsNullOrWhiteSpace(oids.oidWasteContainer))
                        printer.wasteContainer = ParseInt(responses, oids.oidWasteContainer);
                    else
                        printer.wasteContainer = -1;

                    if (!string.IsNullOrWhiteSpace(oids.oidUnitImage) && !string.IsNullOrWhiteSpace(oids.oidUnitImageFULL))
                        printer.unitImage = CalcularPorcentajeToner(responses, oids.oidUnitImage, oids.oidUnitImageFULL);
                    else
                        printer.unitImage = -1;
                }
                else
                {
                    
                    printer.mac = "N/A";
                    printer.serial = "N/A";
                 
                }
            }
        }

        return printersList;
    }



    private Dictionary<string, string> GetSNMPValues(string ipAddress, List<string> oids, int maxRetries = 3)
    {
        var results = new Dictionary<string, string>();
        int retryCount = 0;
        bool success = false;

        while (retryCount < maxRetries && !success)
        {
            try
            {
                var endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), 161);
                var communityParam = new OctetString(community);
                var requestPdu = new List<Variable>();

                foreach (var oid in oids)
                {
                    requestPdu.Add(new Variable(new ObjectIdentifier(oid)));
                }

                var response = Messenger.Get(VersionCode.V2, endpoint, communityParam, requestPdu, timeout);

                foreach (var variable in response)
                {
                    results[variable.Id.ToString()] = variable.Data.ToString();
                }

                success = results.Count > 0;
            }
            catch (SnmpException ex)  // Manejo de errores SNMP específico
            {
                results["Error"] = $"SNMP error: {ex.Message}";
            }
            catch (SocketException ex) // Manejo de problemas de red
            {
                results["Error"] = $"Network error: {ex.Message}";
            }
            catch (Exception ex) // Otras excepciones
            {
                results["Error"] = $"General error: {ex.Message}";
            }

            retryCount++;
            if (!success)
            {
                Thread.Sleep(500); // Esperar antes de reintentar
            }
        }

        return results;
    }

    public bool PingPrinter(string ipAddress)
    {
        try
        {
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(ipAddress, 1000);
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }
    private int CalcularPorcentajeToner(Dictionary<string, string> responses, string oidToner, string oidTonerFull)
    {
        int toner = ParseInt(responses, oidToner);
        int tonerFull = ParseInt(responses, oidTonerFull);

        if (tonerFull > 0)
            return (toner * 100) / tonerFull;
        return 0;
    }
    private int ParseInt(Dictionary<string, string> responses, string oid)
    {
        if (responses.ContainsKey(oid) && int.TryParse(responses[oid].Trim(), out int result))
            return result;
        return 0;
    }
}