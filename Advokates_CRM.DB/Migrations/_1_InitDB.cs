using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.DB.Migrations
{
    [Migration(1)]
    public class _1_InitDB : BaseMigration
    {
        
        public override void Down()
        {
            Execute.Script(scriptsFolder + "\\" + dropScriptFileName);
        }

        public override void Up()
        {
            try
            {
                Execute.Script(scriptsFolder + "\\" + initScriptFileName);

                /*
                                 string sql = File.ReadAllText(scriptsFolder + "\\" + initScriptFileName);
                Execute.Sql(sql);
                 */
            }
            catch (Exception ex)
            { 
            }
        }
    }
}
