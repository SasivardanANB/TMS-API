import React from 'react';
import './custommodal.scss';
import classNames from 'classnames/bind';
import { Modal } from 'reactstrap';

function CustomModal(props) {
    return (
        <Modal isOpen={props.isOpen} toggle={props.onClick} className="modal-dialog-centered custom-modal">
            <div className="modal-header-wrap">
                <div className="modal-header d-inline-block w-auto bg-white">
                    <h5 className="modal-title">{props.modaltitle}</h5>
                </div>
            </div>
            <div className="modal-body bg-white">
                {props.children}
            </div>
        </Modal>
    );
}

export default CustomModal;

{/* <CustomModal isOpen={this.state.showModal} onClick={() => this.switchModal(false)} /> */}