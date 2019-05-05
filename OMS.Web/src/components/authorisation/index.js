import React from 'react';
import './authorisation.scss';

import Tabs from '../common/tabs';

const renderTabs = [
    {
        name: 'User Role',
        component: 'userrole',
        url: 'authorisation/tabs/userrole'
    },
    {
        name: 'Role Mangement',
        component: 'rolemanagement',
        url: 'authorisation/tabs/rolemanagement'
    },
    {
        name: 'User Application',
        component: 'userapplication',
        url: 'authorisation/tabs/userapplication'
    }
]

function Authorisation(props) {
    return (
        <React.Fragment>
            <Tabs tabs={renderTabs} current={props.match.params.tab} onClick={x => props.history.push(`/authorisation/${x}`)} />
        </React.Fragment>
    )
}

export default Authorisation;