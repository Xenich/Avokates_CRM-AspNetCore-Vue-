using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.DB.Migrations
{
    [Migration(3)]
    public class _3_PopulateDB : BaseMigration
    {
        public override void Down()
        {
            Execute.Script(scriptsFolder + "\\" + dropScriptFileName);
            Execute.Script(scriptsFolder + "\\" + initScriptFileName);
        }

        public override void Up()
        {
            try
            {
                Execute.Script(scriptsFolder + "\\" + populateDBScriptFileName);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
