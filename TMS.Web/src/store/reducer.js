import { combineReducers } from 'redux';
import * as ActionType from './types';

const INITIAL_STATE = {
    layout: {
        sidebar: {
            desktop: false,
            mobile: false
        }
    },
    loader: false
};

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

const loaderReducer = (state = INITIAL_STATE.loader, action) => {
    switch (action.type) {
        case ActionType.Show_Loader:
            return true;
        case ActionType.Hide_Loader:
            return false;
        case ActionType.Switch_Loader:
            return action.payload;
        default:
            return state
    }
}

export default combineReducers({
    layout: layoutReducer,
    loader: loaderReducer
});