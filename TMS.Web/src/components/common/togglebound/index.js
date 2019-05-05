import React from 'react';
import './togglebound.scss';
import classNames from 'classnames/bind';

function ToggleBound(props) {
    return (
        <div className={classNames("toggle-wrap", "d-inline-block", {'toggle' : props.toggle})} onClick={() => props.onClick()}>
            <p className={classNames("toggle-placeholder", {'active' : props.toggle})}>INBOUND</p>
            <p className={classNames("toggle-placeholder", {'active' : !props.toggle})}>OUTBOUND</p>
            <div className={classNames("toggle-slider", {'active' : !props.toggle})}></div>
        </div>
    );
}

export default ToggleBound;