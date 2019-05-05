import React from 'react';
import './loader.scss';
import classNames from 'classnames/bind';
import { connect } from 'react-redux';

function Loader(props) {
    return (
        <div className={classNames("loader-wrap", { "d-none": !Boolean(props.loader.length) })}>
            <img width={props.width} height={props.height} alt="" src={require("../../../img/loader.svg")} />
        </div>
    );
}

export const StaticLoader = <div className="text-center">
                                <img width="auto" height="100%" alt="" style={{"maxHeight": "150px"}} src={require("../../../img/loader.svg")} />
                            </div>;

const mapStateToProps = (state) => {
    let { loader } = state;
    return { loader }
};

export default connect(mapStateToProps)(Loader);