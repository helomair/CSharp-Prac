using test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Data;
using Dapper;
using System.ComponentModel;

namespace test.Pages.NewbiePrac
{
    public class Index : PageModel
    {
        public void OnGet()
        {
            dynamic Data = JObject.Parse( this.GetApiData() );
            JArray records = Data.result.records as JArray;

            List<DataStore> ApiRecordData = new List<DataStore>();
            foreach ( JObject record in records ) 
                ApiRecordData.Add(record.ToObject<DataStore>());

            // Insert Data
            this.InsertRecord( DataTableMaker.MakeDataTable(ApiRecordData) );
        }

        public async Task<IActionResult> OnGetQueryAsync() {
            using (SqlConnection conn = new SqlConnection(DBConnectionStr.ConnStr))
            {
                var result = await conn.QueryAsync($"sp_RecordSearch", param: new {}, commandType: CommandType.StoredProcedure );

                var returnData = new JObject
                {
                    ["data"] = JToken.FromObject(result)
                };

                return Content( returnData.ToString(), "application/json");
            }
        }

        private string GetApiData ()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://od.moi.gov.tw/api/v1/rest/datastore/A01010000C-000628-103");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using( HttpWebResponse response = (HttpWebResponse)request.GetResponse() )
            using( Stream stream = response.GetResponseStream() )
            using( StreamReader reader = new StreamReader( stream ) )

            return reader.ReadToEnd();
        }

        private async void InsertRecord ( DataTable InsertDatas ) {
            using (SqlConnection conn = new SqlConnection(DBConnectionStr.ConnStr)) 
            {
                await conn.ExecuteAsync($"sp_RecordInsert", param: new {  
                    RecordGroups = InsertDatas.AsTableValuedParameter("RecordInfo") 
                }, commandType: CommandType.StoredProcedure );
            }
        }
    }
}