using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BeatPulse;
using BeatPulse.Network;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Renci.SshNet;
using Xunit;

namespace FunctionalTests.BeatPulse.Network
{
    [Collection("execution")]
    public class sftp_liveness_should
    {
        private readonly ExecutionFixture _fixture;

        public sftp_liveness_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task be_healthy_when_connection_to_sftp_is_successful_using_password_authentication()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(setup =>
                    {
                        setup.AddSftpLiveness(options =>
                        {
                            var sftpHostConfiguration = new SftpHostConfiguration("localhost", 22);
                            sftpHostConfiguration.UsePasswordAuthentication("foo", "pass");


                            options.AddHost(sftpHostConfiguration);
                        });
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task be_healthy_when_connection_to_sftp_is_successful_using_private_key()
        {
            string privateKey = @"-----BEGIN RSA PRIVATE KEY-----
            Proc-Type: 4,ENCRYPTED
            DEK-Info: AES-128-CBC,615ADC53C7AE4F4DF91AFE9D45AA1120

            M/sXUvoGut2/0CDFojG/HvFmoK5hUadzeIdHh/UN3UWR/t+bSpeqGIPTicftXTqc
            gwgd+Mct6YYL9vNITOwmdF73soniupLsstu6AZLOjo3rQF0frLxOggIWZ/rcsC9Y
            Wcuwcy5LJqLXPkcmHoyCACNlLIobv6L749gtDwLvYwuP9nJF+O0U3x8Orr9E97eb
            KFMMl2WVc+vmu+DLdwBT9feAtwIJZIKaXtXrJtCvjI48tpC6Sj7aGkZBlL8NfhJ4
            jFnepowdkLsLhesRhyPkjYHpeVdL6aJAL55cpS8LJMB7ta9oyOQnPfQ85Ex3E2a4
            aT8+Kg4EcTvAVs8B605BmSyh/nFrMpkwppjtnse4eJ4PklpvvPi8cDiaoyA8gdsk
            hm5xFiEmgaKtSJFbVhmIuqvP4SceyRgy0EekXt/NilQINxRvsAq9dG/fDEdhAduD
            XbYcrbTIYx9aoom2KceHhH373LrHQ/fF46k0yR84KHWakltN7XfbBUMHS83WSJu1
            OLxE7UGeawXtr12IX8TWY0H8siOVKvyjyGpk2dx3lzIlKEJlaxJI1itNprnvb0+V
            njHecCxbhVcQ+O9JAnkTASb2v32HIwWWX/mCq3uFL274hOK/sTsP/ZC5326zyavP
            TJhlfueDsjTZmONGMu27g89sG48KtgKXYFl3OJgWW2Cch9yw4+T+a8mBSb/OtHRM
            Nc8Tm384CzWRlTN/PTkvGHilDpiQlZvMCDaNpO2S2iRIhCK9PW+p/ohc+PQYRo0H
            TnvYI2s1xmHJ0JLh2RfPhSXM4SUnwbNtvr6ER1qS1blGcmureWHDsndquaulK+Oh
            URSAfdCULYJoeMLAmS2Ikh/e0ION2jw/MWAIpB+gsIWaIR9eBXgdsPhdf1Uj4kYx
            KtrcSra2PxZ0j9rkRzIYr8vT3jdUTrkKdVZCndlCspiqAWC9ZMfIwMDm2LQ7kDHu
            2p7ruLq5k5jbI522xK1mLQpQE5IjAshT11Ihz78mEEN4wNhd45cTVyWzaHCRqCAp
            qPnILFBaJXMv9lgo2DSwEcFQ0qc9mMT8d610aT3Ny64oDysmVvg+OF3vMMGY69zj
            Zu4fHtgrXtAdn/WLokg66o89B/yACeleYVcjq3slW+z0Lz76oShfnYK1SqUBT7JN
            yJ+At15Gy2hukTpr4nHY0JSWakuyO3/V+Rm+9xEuHLFd9qhC2fFKNOGOURjnrkr7
            PkKWR1mhRTL0JqJN4PGzb3h3KkLEbC7SXi28SBbRzAO4iygoDLJGCwcgyXR+TtuB
            Yr5XzLSzj1MZ5HFXXb84sMVQbHNEXv5drlU7wrcIfN8EW6+OtKkDOGjM/wkr9zFd
            WQx7hzopHrXu1jlME+RBtOo+S7WlaHhvWNqTBNcee3AsumylhsnYQDusWs4V//em
            CdPmqcV6k2sYgyAWeLc+PQsDsfKgJitVacmc/gtVVDr8Nl3LUFSy2pnTCRm8vQNS
            svsIzMscp+kN/3fEL9LO2AP5jM+5KChH9wQY4Bq/yb7jdDTY5xWG8/3/ye5ZT7yX
            dEvEt71RJuBS6T2/Um+L3LEPc11DyvMkId0dtN+F9ZUipjAJ+TA3qBVrRG4EKqgy
            -----END RSA PRIVATE KEY-----
            ";

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(setup =>
                    {
                        setup.AddSftpLiveness(options =>
                        {
                            var sftpHostConfiguration = new SftpHostConfiguration("localhost", 22);
                            sftpHostConfiguration.UsePrivateKeyAuthentication("foo", privateKey, "beatpulse");
                            options.AddHost(sftpHostConfiguration);
                        });
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest(BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
                .GetAsync();

            response.EnsureSuccessStatusCode();
        }
    }
}
