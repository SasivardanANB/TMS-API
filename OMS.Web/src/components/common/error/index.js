import React from 'react';
import './error.scss';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { modifyerror } from '../../../store/actions';

import { Modal } from 'reactstrap';

function Error(props) {
    let { error } = props;
    return (
        <Modal isOpen={error.show} toggle={() => props.modifyerror({show: false})} className="modal-dialog-centered">
            <div className="modal-body text-center">
                <div className="error-icon"><i className="far fa-times-circle"></i></div>
                <h2 className="modal-title mb-3">{error.heading}</h2>
                <p className="modal-message">{error.text}</p>
                <div className="modal-button-wrap text-right"><button type="button" onClick={() => props.modifyerror({show: false})} className="btn btn-outline-primary">Close</button></div>
            </div>
        </Modal>
    );
}

const mapStateToProps = (state) => {
    let { error } = state;
    return { error }
};

const mapDispatchToProps = dispatch => (
    bindActionCreators({
        modifyerror
    }, dispatch)
);

export default connect(mapStateToProps, mapDispatchToProps)(Error);