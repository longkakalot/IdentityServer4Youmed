using CertificateManager;
using CertificateManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace GetStarted
{
    public class Program
    {
        //private readonly IWebHostEnvironment _environment;

        //public Program(IWebHostEnvironment environment)
        //{
        //    _environment = environment;
        //}

        static CreateCertificates _cc;

        public static X509Certificate2 CreateRsaCertificate(string dnsName, int validityPeriodInYears)
        {
            var basicConstraints = new BasicConstraints
            {
                CertificateAuthority = false,
                HasPathLengthConstraint = false,
                PathLengthConstraint = 0,
                Critical = false
            };

            var subjectAlternativeName = new SubjectAlternativeName
            {
                DnsName = new List<string> { dnsName }
            };

            var x509KeyUsageFlags = X509KeyUsageFlags.DigitalSignature;

            // only if certification authentication is used
            var enhancedKeyUsages = new OidCollection
            {
                new Oid("1.3.6.1.5.5.7.3.1"),  // TLS Server auth
                new Oid("1.3.6.1.5.5.7.3.2"),  // TLS Client auth
            };

            var certificate = _cc.NewRsaSelfSignedCertificate(
                new DistinguishedName { CommonName = dnsName },
                basicConstraints,
                new ValidityPeriod
                {
                    ValidFrom = DateTimeOffset.UtcNow,
                    ValidTo = DateTimeOffset.UtcNow.AddYears(validityPeriodInYears)
                },
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                new RsaConfiguration { KeySize = 2048 }
            );

            return certificate;
        }


        public static void Main(string[] args)
        {

            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .WriteTo.File("log.txt")
                .CreateLogger();

            try
            {
                Log.Information("Starting host...");

                var sp = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

                _cc = sp.GetService<CreateCertificates>();

                var rsaCert = CreateRsaCertificate("localhost", 10);
                //var ecdsaCert = CreateECDsaCertificate("localhost", 10);

                string password = "1234";
                var iec = sp.GetService<ImportExportCertificate>();

                var rsaCertPfxBytes =
                    iec.ExportSelfSignedCertificatePfx(password, rsaCert);
                File.WriteAllBytes("rsaCert.pfx", rsaCertPfxBytes);


                CreateHostBuilder(args).Build().Run();
                //return ""0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                //return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            

            //var ecdsaCertPfxBytes =
            //    iec.ExportSelfSignedCertificatePfx(password, ecdsaCert);
            //File.WriteAllBytes("ecdsaCert.pfx", ecdsaCertPfxBytes);
            //CreateHostBuilder(args).Build().Run();
        }

        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
