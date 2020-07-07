﻿using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Addons.Interactive
{
    /// <summary>
    /// The interactive base.
    /// </summary>
    public class InteractiveBase : InteractiveBase<SocketCommandContext>
    {
    }

    /// <summary>
    /// The interactive base.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class InteractiveBase<T> : ModuleBase<T>
        where T : SocketCommandContext
    {
        /// <summary>
        /// Gets or sets the interactive service.
        /// </summary>
        public InteractiveService Interactive { get; set; }

        public Task<SocketMessage> NextMessageAsync(ICriterion<SocketMessage> criterion, TimeSpan? timeout = null)
            => Interactive.NextMessageAsync(Context, criterion, timeout);

        public Task<SocketMessage> NextMessageAsync(bool fromSourceUser = true, bool inSourceChannel = true, TimeSpan? timeout = null)
            => Interactive.NextMessageAsync(Context, fromSourceUser, inSourceChannel, timeout);

        public Task<IUserMessage> ReplyAndDeleteAsync(string content, bool isTTS = false, Embed embed = null, TimeSpan? timeout = null, RequestOptions options = null)
            => Interactive.ReplyAndDeleteAsync(Context, content, isTTS, embed, timeout, options);

        public Task<IUserMessage> InlineReactionReplyAsync(ReactionCallbackData data, bool fromSourceUser = true)
            => Interactive.SendMessageWithReactionCallbacksAsync(Context, data, fromSourceUser);

        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, ReactionList reactions, bool fromSourceUser = true)
        {
            var criterion = new Criteria<SocketReaction>();
            if (fromSourceUser)
                criterion.AddCriterion(new EnsureReactionFromSourceUserCriterion());
            return PagedReplyAsync(pager, criterion, reactions);
        }

        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, ICriterion<SocketReaction> criterion, ReactionList reactions)
            => Interactive.SendPaginatedMessageAsync(Context, pager, reactions, criterion);

        public RuntimeResult Ok(string reason = null) => new OkResult(reason);
    }
}