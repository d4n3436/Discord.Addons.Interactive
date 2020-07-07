﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Addons.Interactive
{
    /// <summary>
    /// The inline reaction callback.
    /// </summary>
    public class InlineReactionCallback : IReactionCallback
    {
        /// <summary>
        /// The interactive service.
        /// </summary>
        private readonly InteractiveService interactive;

        /// <summary>
        /// The callback data.
        /// </summary>
        private readonly ReactionCallbackData data;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineReactionCallback"/> class.
        /// </summary>
        /// <param name="interactive">
        /// The interactive service.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="data">
        /// The callback data.
        /// </param>
        /// <param name="criterion">
        /// The criterion.
        /// </param>
        public InlineReactionCallback(
            InteractiveService interactive,
            SocketCommandContext context,
            ReactionCallbackData data,
            ICriterion<SocketReaction> criterion = null)
        {
            this.interactive = interactive;
            Context = context;
            this.data = data;
            Criterion = criterion ?? new EmptyCriterion<SocketReaction>();
            Timeout = data.Timeout ?? TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// The run mode.
        /// </summary>
        public RunMode RunMode => RunMode.Sync;

        /// <summary>
        /// Gets the criterion.
        /// </summary>
        public ICriterion<SocketReaction> Criterion { get; }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        public TimeSpan? Timeout { get; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public SocketCommandContext Context { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public IUserMessage Message { get; private set; }

        /// <summary>
        /// Sends the message and adds the reaction callback.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task DisplayAsync()
        {
            var message = await Context.Channel.SendMessageAsync(data.Text, embed: data.Embed).ConfigureAwait(false);
            Message = message;
            interactive.AddReactionCallback(message, this);

            _ = Task.Run(async () =>
            {
                foreach (var item in data.Callbacks)
                {
                    await message.AddReactionAsync(item.Reaction);
                }
            });

            if (Timeout.HasValue)
            {
                _ = Task.Delay(Timeout.Value)
                    .ContinueWith(_ =>
                        {
                            interactive.RemoveReactionCallback(message);
                            data.TimeoutCallback?.Invoke(Context);
                        });
            }
        }

        /// <summary>
        /// Handle the reaction callbacks.
        /// </summary>
        /// <param name="reaction">
        /// The reaction.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            // If reaction is not specified in our Callback List, ignore
            var reactionCallbackItem = data.Callbacks.FirstOrDefault(t => t.Reaction.Equals(reaction.Emote));
            if (reactionCallbackItem == null)
            {
                return false;
            }

            if (data.SingleUsePerUser)
            {
                // Ensure that we only allow users to react a single time.
                if (!data.ReactorIDs.Contains(reaction.UserId))
                {
                    await reactionCallbackItem.Callback(Context, reaction);
                    data.ReactorIDs.Add(reaction.UserId);
                }
            }
            else
            {
                await reactionCallbackItem.Callback(Context, reaction);
            }

            return data.ExpiresAfterUse;
        }
    }
}