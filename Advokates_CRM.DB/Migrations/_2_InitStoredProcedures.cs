using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.DB.Migrations
{
    [Migration(2)]
    public class _2_InitStoredProcedures : BaseMigration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            try
            {
                Execute.Script(scriptsFolder + "\\" + storedProceduresScriptFileName);
            }
            catch (Exception ex)
            {
            }
        }
    }
}