export const INITIAL_STATE = {
    credentials: {
        token: "/LN4/q3IvTn8oTMfqKQYDHMAESsh5mSpyu/elNUfIubEq9g9u0mT47++rllXRcMq"
    },
    error: {
        show: false,
        heading: "Something went wrong!",
        text: "Please try again later. We apologize for the inconvinience caused."
    },
    layout: {
        sidebar: {
            desktop: false,
            mobile: false
        }
    },
    loader: [],
    views: {
        authorisation: {
            userRoleTable: {
                PageNumber: 1,
                NumberOfRecords: 0,
                PageSize: 10,
                SortOrder: "username",
                Data: []
            },
            userRoleData: {
                users: [],
                roles: [],
                regions: []
            },
            rolesMngtTable: {
                PageNumber: 1,
                NumberOfRecords: 0,
                PageSize: 10,
                SortOrder: "rolecode",
                Data: []
            },
            menuActivities: [],
            userAppTable: {
                PageNumber: 1,
                NumberOfRecords: 0,
                PageSize: 10,
                SortOrder: "username",
                Data: []
            },
            applications: []
        }
    }
};