import * as ActionType from '../types';
import axios from 'axios';

import { rootURL, ops } from '../../config';

import { addloader, removeloader, modifyerror } from '../actions';

/* USER ROLE */

// Actions

export const modifyuserroletable = val => (
    {
        type: ActionType.Modify_User_Role_Table,
        payload: val
    }
);

export const modifyuserroledata = val => (
    {
        type: ActionType.Modify_User_Role_Data,
        payload: val
    }
);

// Thunk
export const getuserroles = body => {
    return async function (dispatch, getState) {
        dispatch(addloader('getuserroles'));

        await axios(rootURL + ops.users.getuserroles, {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                "Token": getState().credentials.token
            },
            data: JSON.stringify(body)
        })
            .then(function (response) {
                if (response.statusText === "OK" && (response.data.Status === "Success" || (response.data.Status === "Failure" && response.data.NumberOfRecords === 0))) {
                    let { NumberOfRecords, Data } = response.data,
                        { PageNumber, PageSize, SortOrder } = body;
                    dispatch(modifyuserroletable({ NumberOfRecords, Data, PageNumber, PageSize, SortOrder }));
                }
                else {
                    console.log("response", response);
                    dispatch(modifyerror({ show: true }));
                }
            })
            .catch(function (error) {
                dispatch(modifyerror({ show: true }));
                console.log("error", error)
            });

        dispatch(removeloader('getuserroles'));
    };
}

export const getuserrolesdata = () => {
    return async function (dispatch, getState) {
        dispatch(addloader('getuserrolesdata'));

        let users = [],
            roles = [],
            regions = [];

        await Promise.all([
            axios(rootURL + ops.users.getusernames, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                    "Token": getState().credentials.token
                }
            })
                .then(function (response) {
                    if (response.statusText === "OK" && (response.data.Status === "Success" || (response.data.Status === "Failure" && response.data.NumberOfRecords === 0))) {
                        users = response.data.Data;
                    }
                    else {
                        console.log("response", response);
                        dispatch(modifyerror({ show: true }));
                    }
                })
                .catch(function (error) {
                    dispatch(modifyerror({ show: true }));
                    console.log("error", error)
                }),
            axios(rootURL + ops.users.getrolecodes, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                    "Token": getState().credentials.token
                }
            })
                .then(function (response) {
                    if (response.statusText === "OK" && (response.data.Status === "Success" || (response.data.Status === "Failure" && response.data.NumberOfRecords === 0))) {
                        roles = response.data.Data;
                    }
                    else {
                        console.log("response", response);
                        dispatch(modifyerror({ show: true }));
                    }
                })
                .catch(function (error) {
                    dispatch(modifyerror({ show: true }));
                    console.log("error", error)
                }),
            axios(rootURL + ops.users.getregioncodes, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                    "Token": getState().credentials.token
                }
            })
                .then(function (response) {
                    if (response.statusText === "OK" && (response.data.Status === "Success" || (response.data.Status === "Failure" && response.data.NumberOfRecords === 0))) {
                        regions = response.data.Data;
                    }
                    else {
                        console.log("response", response);
                        dispatch(modifyerror({ show: true }));
                    }
                })
                .catch(function (error) {
                    dispatch(modifyerror({ show: true }));
                    console.log("error", error)
                })
        ]);

        dispatch(modifyuserroledata({users: users, roles: roles, regions: regions}));

        dispatch(removeloader('getuserrolesdata'));
    };
}

/* ROLES MANAGEMENT */

// Actions

export const modifyrolesmngttable = val => (
    {
        type: ActionType.Modify_Roles_Mngt_Table,
        payload: val
    }
);

export const modifymenuactivities = val => (
    {
        type: ActionType.Modify_Menu_Activities,
        payload: val
    }
);

// Thunk
export const getroles = body => {
    return async function (dispatch, getState) {
        dispatch(addloader('getroles'));

        await axios(rootURL + ops.users.getroles, {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                "Token": getState().credentials.token
            },
            data: JSON.stringify(body)
        })
            .then(function (response) {
                if (response.statusText === "OK" && (response.data.Status === "Success" || (response.data.Status === "Failure" && response.data.NumberOfRecords === 0))) {
                    let { NumberOfRecords, Data } = response.data,
                        { PageNumber, PageSize, SortOrder } = body;
                    dispatch(modifyrolesmngttable({ NumberOfRecords, Data, PageNumber, PageSize, SortOrder }));
                }
                else {
                    console.log("response", response);
                    dispatch(modifyerror({ show: true }));
                }
            })
            .catch(function (error) {
                dispatch(modifyerror({ show: true }));
                console.log("error", error)
            });

        dispatch(removeloader('getroles'));
    };
}

export const getmenuactivities = () => {
    return async function (dispatch, getState) {
        dispatch(addloader('getmenuactivities'));

        await axios(rootURL + ops.users.getmenuwithactivities, {
            method: 'GET',
            headers: {
                "Content-Type": "application/json",
                "Token": getState().credentials.token
            }
        })
            .then(function (response) {
                if (response.statusText === "OK" && (response.data.Status === "Success" || (response.data.Status === "Failure" && response.data.NumberOfRecords === 0))) {
                    dispatch(modifymenuactivities(response.data.Data));
                }
                else {
                    console.log("response", response);
                    dispatch(modifyerror({ show: true }));
                }
            })
            .catch(function (error) {
                dispatch(modifyerror({ show: true }));
                console.log("error", error)
            });

        dispatch(removeloader('getmenuactivities'));
    };
}

/* USER APPLICATION */

// Actions

export const modifyuserapptable = val => (
    {
        type: ActionType.Modify_User_App_Table,
        payload: val
    }
);

// Thunk
export const getusers = body => {
    return async function (dispatch, getState) {
        dispatch(addloader('getusers'));

        await axios(rootURL + ops.users.getusers, {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                "Token": getState().credentials.token
            },
            data: JSON.stringify(body)
        })
            .then(function (response) {
                if (response.statusText === "OK" && (response.data.Status === "Success" || (response.data.Status === "Failure" && response.data.NumberOfRecords === 0))) {
                    let { NumberOfRecords, Data } = response.data,
                        { PageNumber, PageSize, SortOrder } = body;
                    dispatch(modifyuserapptable({ NumberOfRecords, Data, PageNumber, PageSize, SortOrder }));
                }
                else {
                    console.log("response", response);
                    dispatch(modifyerror({ show: true }));
                }
            })
            .catch(function (error) {
                dispatch(modifyerror({ show: true }));
                console.log("error", error)
            });

        dispatch(removeloader('getusers'));
    };
}

/* Applications */

// Actions

export const addapplications = val => (
    {
        type: ActionType.Add_Applications,
        payload: val
    }
);

// Thunk
export const getapplications = () => {
    return async function (dispatch, getState) {
        dispatch(addloader('getapplications'));

        await axios(rootURL + ops.users.getapplications, {
            method: 'GET',
            headers: {
                "Content-Type": "application/json",
                "Token": getState().credentials.token
            }
        })
            .then(function (response) {
                console.log(response);
                if (response.statusText === "OK" && (response.data.Status === "Success" || (response.data.Status === "Failure" && response.data.NumberOfRecords === 0))) {
                    dispatch(addapplications(response.data.Data));
                }
                else {
                    console.log("response", response);
                    dispatch(modifyerror({ show: true }));
                }
            })
            .catch(function (error) {
                dispatch(modifyerror({ show: true }));
                console.log("error", error)
            });

        dispatch(removeloader('getapplications'));
    };
}