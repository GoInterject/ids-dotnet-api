using System.ComponentModel.DataAnnotations;

namespace Interject.Api
{
    public class PassThroughCommand
    {
        public string ConnectionStringName { get; set; }

        /// <summary>
        /// This property is not used.
        /// </summary>
        public CommandType CommandType { get; set; }

        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public System.Data.CommandType GetCommandType()
        {
            if (this.CommandType == CommandType.TableDirect)
            {
                return System.Data.CommandType.TableDirect;
            }
            else if (this.CommandType == CommandType.Text)
            {
                return System.Data.CommandType.Text;
            }
            else
            {
                return System.Data.CommandType.StoredProcedure;
            }
        }
    }
}