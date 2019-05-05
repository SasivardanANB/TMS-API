import React from 'react';
import './downloadorder.scss';

import ToggleBound from '../common/togglebound';
import Form from '../common/form';
import "react-datepicker/dist/react-datepicker.css";

let uploadFormElems = [
    {
        name: 'Tanggal Order',
        placeholder: 'From Date',
        value: new Date(),
        errMsg: '',
        required: true,
        valid: false,
        field: {
            type: "datepicker"
        },
        gridClass: "col-12 col-sm-6 col-md-4 col-lg-3"
    },
    {
        name: 'Tanggal Order 2',
        placeholder: 'From Date',
        value: new Date(),
        errMsg: '',
        required: true,
        valid: false,
        field: {
            type: "datepicker"
        },
        gridClass: "col-12 col-sm-6 col-md-4 col-lg-3 hide-label"
    },
    {
        name: 'Status Reservasi',
        value: '',
        errMsg: '',
        required: true,
        valid: false,
        field: {
            type: "select",
            options: [
                {
                    label: 'Status Reservasi',
                    value: ''
                },
                {
                    label: 'option 1',
                    value: '1'
                },
                {
                    label: 'option 2',
                    value: '2'
                },
                {
                    label: 'option 3',
                    value: '3'
                }
            ]
        },
        gridClass: "col-12 col-sm-6 col-md-4 col-lg-6",
        check: [
            {
                regex: /^[0-9]$/,
                message: "Please select an option"
            }
        ]
    },
    {
        name: 'No. Order',
        placeholder: 'Masukkan nomor order',
        value: '',
        errMsg: '',
        required: true,
        valid: false,
        field: {
            type: "text"
        },
        gridClass: "col-12 col-md-6 col-lg-6",
        check: [
            {
                regex: /^.{3,30}$/,
                message: "Should be 3 - 30 characters"
            },
            {
                regex: /^[a-zA-Z]{3,30}$/,
                message: "First name should not have any special characters, numerics or spaces"
            }
        ]
    }
];

class DownloadOrder extends React.Component {
    constructor(props) {
        super(props);
        this.state = { inbound: true, startDate: new Date() }
    }

    handleChange(date) {
        this.setState({
            startDate: date
        });
    }

    modalFormSubmit(data) {
        console.log(data);
    }

    render() {
        return (
            <React.Fragment>
                <div className="text-right">
                    <ToggleBound toggle={this.state.inbound} onClick={() => this.setState({ inbound: !this.state.inbound })} />
                </div>
                <div className="tabs-wrap">
                    <div className="tabs-header-wrap">
                        <div className="tabs-title d-none d-md-block d-lg-block active">Download Order</div>
                        <div className="clearfix"></div>
                    </div>
                    <div className="tabs-content">
                        <Form
                            fields={uploadFormElems}
                            className="upload-form px-2"
                            footerClassName="col-12"
                            formButtons={<button className="text-uppercase btn btn-primary search-button px-5 mt-0" type="submit">Search</button>}
                            onSubmit={obj => this.modalFormSubmit(obj)}
                            ref="formRef"
                        />
                        <hr className="row mt-5" />
                        <div className="search-results-wrap">
                            <h6 className="px-2 font-weight-bold m-0">Search Result</h6>
                            <div className="py-4 row px-2">
                                <div className="col-6">
                                    <button type="button" className="btn btn-primary search-result-button w-100 d-flex">
                                        <span className="button-title">TMS-28032019-inbound.xls</span>
                                        <i className="far fa-arrow-alt-circle-down"></i>
                                    </button>
                                </div>
                                <div className="col-6">
                                    <button type="button" className="btn btn-primary search-result-button w-100 d-flex">
                                        <span className="button-title">TMS-01042019-inbound.xls</span>
                                        <i className="far fa-arrow-alt-circle-down"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </React.Fragment>
        );
    }
}

export default DownloadOrder;