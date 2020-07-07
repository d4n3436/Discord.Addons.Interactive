using System.Threading.Tasks;
using Discord.Commands;

namespace Discord.Addons.Interactive
{
    /// <summary>
    /// A criterion that is always successful.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public class EmptyCriterion<T> : ICriterion<T>
    {
        /// <inheritdoc/>
        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, T parameter)
            => Task.FromResult(true);
    }
}