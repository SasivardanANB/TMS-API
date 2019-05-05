export const
    rootURL = "http://tms.anblicks.com:80/oms-qa-api/api/v1/",
    ops = {
        users: {
            getuserroles: "user/getuserroles",
            getusernames: "user/getusernames",
            getrolecodes: "user/getrolecodes",
            getregioncodes: "user/getregioncodes",
            createupdateuserrole: "user/createupdateuserrole",
            deleteuserrole: "user/deleteuserrole",
            getroles: "user/getroles",
            getmenuwithactivities: "user/getmenuwithactivities",
            GetRoleDetails: "user/GetRoleDetails",
            createupdaterole: "user/createupdaterole",
            deleterole: "user/deleterole",
            getusers: "user/getusers",
            getapplications: "user/getapplications",
            createupdateuser: "user/createupdateuser",
            deleteuser: "user/deleteuser"
        }
    },
    entriesPerPage = [5, 10, 20, 50];