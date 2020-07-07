using System.Threading.Tasks;
using Discord.Commands;

namespace Discord.Addons.Interactive
{
    public interface ICriterion<in T>
    {
        /// <summary>
        /// Ensures that all the criteria are successful.
        /// </summary>
        /// <param name="sourceContext">
        /// The source context.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> JudgeAsync(SocketCommandContext sourceContext, T parameter);
    }
}