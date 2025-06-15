using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;

[RoutePrefix("api/ledgermaster")]
public class LedgerMasterController : ApiController
{
    string conStr = ConfigurationManager.ConnectionStrings["ConApplication"].ConnectionString;

    // Get All Ledgers
    [HttpGet]
    [Route("list")]
    public IHttpActionResult GetLedgerList()
    {
        return Ok(ExecuteStoredProcedure("SELECT"));
    }

    // Get Genders
    [HttpGet]
    [Route("gender")]
    public IHttpActionResult GetGender()
    {
        return Ok(ExecuteStoredProcedure("GET_GENDER"));
    }

    // Get States
    [HttpGet]
    [Route("state")]
    public IHttpActionResult GetState()
    {
        return Ok(ExecuteStoredProcedure("GET_STATE"));
    }

    // Get Cities by StateId
    [HttpGet]
    [Route("city/{stateId}")]
    public IHttpActionResult GetCityByState(int stateId)
    {
        return Ok(ExecuteStoredProcedure("GET_CITY", new SqlParameter("@State_Id", stateId)));
    }

    // Insert or Update Ledger
    [HttpPost]
    [Route("save")]
    public IHttpActionResult SaveLedger(LedgerModel model)
    {
        string action = model.Ledger_Id > 0 ? "UPDATE" : "INSERT";

        List<SqlParameter> parms = new List<SqlParameter>
    {
        new SqlParameter("@Ledger_Id", model.Ledger_Id),
        new SqlParameter("@Name", model.Name),
        new SqlParameter("@Gender_Id", model.Gender_Id),
        new SqlParameter("@ContactNo", model.ContactNo),
        new SqlParameter("@Address1", model.Address1),
        new SqlParameter("@Address2", model.Address2),
        new SqlParameter("@State_Id", model.State_Id),
        new SqlParameter("@City_Id", model.City_Id),
        new SqlParameter("@PinCode", model.PinCode)
    };

        ExecuteStoredProcedure(action, parms);

        return Ok(new { success = true });
    }




    //[HttpPost]
    //[Route("save")]
    //public IHttpActionResult SaveLedger(LedgerModel model)
    //{
    //    string action = model.Ledger_Id > 0 ? "UPDATE" : "INSERT";

    //    List<SqlParameter> parms = new List<SqlParameter>
    //    {
    //        new SqlParameter("@Action", action),
    //        new SqlParameter("@Ledger_Id", model.Ledger_Id),
    //        new SqlParameter("@Name", model.Name),
    //        new SqlParameter("@Gender_Id", model.Gender_Id),
    //        new SqlParameter("@ContactNo", model.ContactNo),
    //        new SqlParameter("@Address1", model.Address1),
    //        new SqlParameter("@Address2", model.Address2),
    //        new SqlParameter("@State_Id", model.State_Id),
    //        new SqlParameter("@City_Id", model.City_Id),
    //        new SqlParameter("@PinCode", model.PinCode)
    //    };

    //    ExecuteStoredProcedure("INSERT", parms); // insert/update both handled
    //    return Ok(new { success = true });
    //}

    // Soft Delete
    [HttpPost]
    [Route("delete/{id}")]
    public IHttpActionResult DeleteLedger(int id)
    {
        return Ok(ExecuteStoredProcedure("DELETE", new SqlParameter("@Ledger_Id", id)));
    }

    // Common Method to call SP
    private DataTable ExecuteStoredProcedure(string action, params SqlParameter[] parameters)
    {
        DataTable dt = new DataTable();

        using (SqlConnection con = new SqlConnection(conStr))
        using (SqlCommand cmd = new SqlCommand("sp_LedgerMaster", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Action", action));
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }

        return dt;
    }
    //private DataTable ExecuteStoredProcedure(string action, List<SqlParameter> parms, params SqlParameter[] parameters)
    //{
    //    DataTable dt = new DataTable();

    //    using (SqlConnection con = new SqlConnection(conStr))
    //    using (SqlCommand cmd = new SqlCommand("sp_LedgerMaster", con))
    //    {
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.Parameters.Add(new SqlParameter("@Action", action));
    //        if (parameters != null)
    //            cmd.Parameters.AddRange(parameters);

    //        SqlDataAdapter da = new SqlDataAdapter(cmd);
    //        da.Fill(dt);
    //    }

    //    return dt;
    //}   

    private DataTable ExecuteStoredProcedure(string action, List<SqlParameter> parameters)
    {
        DataTable dt = new DataTable();

        using (SqlConnection con = new SqlConnection(conStr))
        using (SqlCommand cmd = new SqlCommand("sp_LedgerMaster", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Action", action));
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }

        return dt;
    }


    public class LedgerModel
    {
        public long Ledger_Id { get; set; }
        public string Name { get; set; }
        public int Gender_Id { get; set; }
        public string ContactNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int State_Id { get; set; }
        public int City_Id { get; set; }
        public string PinCode { get; set; }
    }


}