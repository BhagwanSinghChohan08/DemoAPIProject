document.addEventListener("DOMContentLoaded", function () {
    loadGenders();
    loadStates();
    //loadLedgerList();
    loadLedgerTable();
    var isDrpLoaded = 0;

    document.getElementById("ddlState").addEventListener("change", () => {
        let stateId = document.getElementById("ddlState").value;
        loadCities(stateId);
        isDrpLoaded = 1;
    });

    let stateId = document.getElementById("ddlState").value;
    if (stateId > 0 && isDrpLoaded == 0)
        loadCities(stateId);
});

function loadGenders() {
    axios.get('/api/ledgermaster/gender')
        .then(function (response) {
            let ddl = document.getElementById("ddlGender");
            response.data.forEach(function (item) {
                ddl.options.add(new Option(item.Gender_Name, item.Gender_Id));
            });
        })
        .catch(function (error) {
            console.error("Error loading genders:", error);
        });
}

function loadStates() {
    const ddlState = document.getElementById("ddlState");
    ddlState.innerHTML = '<option value="">-- Select State --</option>';

    axios.get('/api/ledgermaster/state')
        .then(function (response) {
            let ddl = document.getElementById("ddlState");
            response.data.forEach(function (item) {
                ddl.options.add(new Option(item.State_Name, item.State_Id));
            });
        })
        .catch(function (error) {
            console.error("Error loading states:", error);
        });
}

function loadCities(stateId) {
    const ddlCity = document.getElementById("ddlCity");
    ddlCity.innerHTML = '<option value="">-- Select City --</option>';

    if (stateId) {
        axios.get(`/api/ledgermaster/city/${stateId}`)
            .then(function (response) {
                response.data.forEach(function (item) {
                    ddlCity.options.add(new Option(item.City_Name, item.City_Id));
                });
            })
            .catch(function (error) {
                console.error("Error loading cities:", error);
            });
    }
}

function saveLedger() {
    let obj = {
        Name: document.getElementById("txtName").value,
        Gender_Id: document.getElementById("ddlGender").value,
        ContactNo: document.getElementById("txtContact").value,
        Address1: document.getElementById("txtAddress1").value,
        Address2: document.getElementById("txtAddress2").value,
        State_Id: document.getElementById("ddlState").value,
        City_Id: document.getElementById("ddlCity").value,
        PinCode: document.getElementById("txtPinCode").value
    };

    axios.post("/api/ledgermaster/save", obj)
        .then(() => {
            alert("Saved successfully");
            resetForm();
            loadLedgerList();
        });
}

function resetForm() {
    document.getElementById("ledgerForm").reset();
}

function loadLedgerTable() {
    axios.get('/api/ledgermaster/list')
        .then(function (response) {
            const data = response.data;
            const tbody = document.getElementById("ledgerTableBody");
            tbody.innerHTML = "";

            data.forEach((item, index) => {
                const tr = document.createElement("tr");
                tr.innerHTML = `
                    <td>${index + 1}</td>
                    <td>${item.Name}</td>
                    <td>${item.Gender}</td>
                    <td>${item.ContactNo}</td>
                    <td>${item.Address1} ${item.Address2}</td>
                    <td>${item.City}</td>
                    <td>${item.State}</td>
                    <td>${item.PinCode}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary" onclick="editLedger(${item.Ledger_Id})"><i class="fa fa-edit"></i></button>
                        <button class="btn btn-sm btn-outline-danger" onclick="deleteLedger(${item.Ledger_Id})"><i class="fa fa-delete"></i></button>
                    </td>
                `;
                tbody.appendChild(tr);
            });
        })
        .catch(function (error) {
            console.error("Error loading ledger data:", error);
        });
}