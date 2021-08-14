using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.DB.Migrations
{
    public abstract class BaseMigration : Migration
    {
        public static string scriptsFolder;
        protected const string dropScriptFileName = "DropDB.sql";
        protected const string initScriptFileName = "initDBScript.sql";
        protected const string populateDBScriptFileName = "PopulateDB.sql";
        protected const string storedProceduresScriptFileName = "StoredProceduresScript.sql";
    }
}
