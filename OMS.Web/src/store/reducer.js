import { combineReducers } from 'redux';
import * as ActionType from './types';
import * as AuthorisationReducer from './viewsreducers/authourisation';
import { INITIAL_STATE } from './initailstate';

/* CREDENTIALS */

const credentialsReducer = (state = INITIAL_STATE.credentials, action) => {
    return state
}

/* ERROR */

const errorReducer = (state = INITIAL_STATE.error, action) => {
    switch (action.type) {
        case ActionType.Modify_Error:
            return Object.assign({}, INITIAL_STATE.error, action.payload);
        default:
            return state
    }
}

/* LAYOUT */

const layoutReducer = (state = INITIAL_STATE.layout, action) => {
    let { sidebar } = state;
    switch (action.type) {
        case ActionType.Switch_Desk_Sidebar:
            return Object.assign({}, state, { sidebar: Object.assign({}, sidebar, { desktop: !sidebar.desktop }) });
        case ActionType.Switch_Mob_Sidebar:
            return Object.assign({}, state, { sidebar: Object.assign({}, sidebar, { mobile: !sidebar.mobile }) });
        default:
            return state
    }
};

/* LOADER */

const loaderReducer = (state = INITIAL_STATE.loader, action) => {
    switch (action.type) {
        case ActionType.Add_Loader:
            return [...state, action.payload]
        case ActionType.Remove_Loader:
            return state.filter(x => (x !== action.payload))
        default:
            return state
    }
}

/* VIEWS */

const viewsReducer = (state = INITIAL_STATE.views, action) => {
    return ({
        authorisation: {
            userRoleTable: AuthorisationReducer.userRoleTable(state.authorisation.userRoleTable, action),
            userRoleData: AuthorisationReducer.userRoleData(state.authorisation.userRoleData, action),
            rolesMngtTable: AuthorisationReducer.rolesMngtTable(state.authorisation.rolesMngtTable, action),
            menuActivities: AuthorisationReducer.menuActivities(state.authorisation.menuActivities, action),
            userAppTable: AuthorisationReducer.userAppTable(state.authorisation.userAppTable, action),
            applications: AuthorisationReducer.applications(state.authorisation.applications, action)
        }
    })
}

export default combineReducers({
    credentials: credentialsReducer,
    error: errorReducer,
    layout: layoutReducer,
    loader: loaderReducer,
    views: viewsReducer
});