using JackboxGPT3.Games.Common;
using JackboxGPT3.Games.JokeBoat.Models;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Games.JokeBoat
{
    public class JokeBoatClient : BcSerializedClient<JokeBoatRoom, JokeBoatPlayer>
    {
        public JokeBoatClient(IConfigurationProvider configuration, ILogger logger, int instance) : base(configuration, logger, instance) { }
    }
}