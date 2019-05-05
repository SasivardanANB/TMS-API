import React from 'react';
import './loader.scss';
import classNames from 'classnames/bind';
import { connect } from 'react-redux';

function Loader(props) {
    return (
        <div className={classNames("loader-wrap", {"d-none": !props.loader})}>
            <img width={props.width} height={props.height} src={require("../../../img/loader.svg")} />
        </div>
    );
}

const mapStateToProps = (state) => {
    const { loader } = state
    return { loader }
};

export default connect(mapStateToProps)(Loader);