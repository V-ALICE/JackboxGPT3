﻿using System.Linq;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.Quiplash1;
using JackboxGPT3.Games.Quiplash1.Models;
using JackboxGPT3.Services;
using Serilog;
using RoomState = JackboxGPT3.Games.Quiplash1.Models.RoomState;

namespace JackboxGPT3.Engines
{
    public class Quiplash1Engine : BaseQuiplashEngine<Quiplash1Client>
    {
        protected override string Tag => "quiplash";

        public Quiplash1Engine(ICompletionService completionService, ILogger logger, Quiplash1Client client, int instance)
            : base(completionService, logger, client, instance)
        {
            JackboxClient.OnSelfUpdate += OnSelfUpdate;
            JackboxClient.OnRoomUpdate += OnRoomUpdate;
            JackboxClient.Connect();
        }

        private void OnSelfUpdate(object sender, Revision<Quiplash1Player> revision)
        {
            if (revision.New.State == RoomState.Gameplay_AnswerQuestion && revision.New.Question != null)
                SubmitQuip(revision.New);
            else if (revision.New.State == RoomState.Gameplay_Vote && !revision.New.ActuallyDoneVoting)
                VoteQuip(revision.New);
        }

        private void OnRoomUpdate(object sender, Revision<Quiplash1Room> revision)
        {
            if (revision.Old.State != revision.New.State)
                LogDebug($"New room state: {revision.New.State}", true);
        }

        private async void VoteQuip(Quiplash1Player self)
        {
            var room = JackboxClient.GameState.Room;
            if (room.Choices == null || room.Choices.Count == 0) return;
            
            var choices = room.GetResponseChoices(self.Ignore); // (list of keys, list of quips)
            var favoriteIdx = await FormVote(room.Question.Prompt, choices.Item2);
            JackboxClient.ChooseFavorite(choices.Item1[favoriteIdx]);
        }

        private async void SubmitQuip(Quiplash1Player self)
        {
            if (self.Question is not { } question) return;

            var quip = await FormQuip(question.Prompt);
            if (quip == "")
            {
                LogInfo("GPT failed to come up with a response, using default response");
                quip = $"AI {Instance} failed to formulate an answer";
            }

            JackboxClient.SubmitQuip(question.ID, quip);
        }

    }
}
