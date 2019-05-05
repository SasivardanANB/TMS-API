import * as ActionType from './types';

/* LAYOUT */

    // Sidebar

    export const switchdesksidebar = () => (
        {
            type: ActionType.Switch_Desk_Sidebar
        }
    );

    export const switchmobsidebar = () => (
        {
            type: ActionType.Switch_Mob_Sidebar
        }
    );

/* LOADER */

export const showloader = () => (
    {
        type: ActionType.Show_Loader
    }
);

export const hideloader = () => (
    {
        type: ActionType.Hide_Loader
    }
);

export const switchloader = val => (
    {
        type: ActionType.Switch_Loader,
        payload: val
    }
);