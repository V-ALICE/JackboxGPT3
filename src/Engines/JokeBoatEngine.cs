using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.JokeBoat;
using JackboxGPT3.Games.JokeBoat.Models;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Engines
{
    public class JokeBoatEngine : BaseJackboxEngine<JokeBoatClient>
    {
        protected override string Tag => "jokeboat";

        public JokeBoatEngine(ICompletionService completionService, ILogger logger, IConfigurationProvider configuration,
            JokeBoatClient client, int instance) : base(completionService, logger, client, instance)
        {
            JackboxClient.OnSelfUpdate += OnSelfUpdate;
            JackboxClient.OnRoomUpdate += OnRoomUpdate;
            JackboxClient.Connect();
        }

        private void OnSelfUpdate(object sender, Revision<JokeBoatPlayer> revision)
        {
        }

        private void OnRoomUpdate(object sender, Revision<JokeBoatRoom> revision)
        {
        }
    }
}