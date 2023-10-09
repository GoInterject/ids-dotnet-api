namespace Interject.Api
{
    public class PassThroughCommand
    {
        /// <summary>
        /// The name of the connection string in Configurations
        /// </summary>
        public string ConnectionStringName { get; set; }

        /// <summary>
        /// The type of this command (none, stored procedure, table, or text). Currently only stored procedure is supported.
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// The name of the command (stored procedure)
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// The time that this command will timeout if not returning successfully
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// Gets this command's type
        /// </summary>
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