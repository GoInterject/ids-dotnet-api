using Interject.Classes;

namespace Interject.Models
{
    public class PassThroughCommand
    {
        public string ConnectionStringName { get; set; }
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