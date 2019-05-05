import React from 'react';
import './uploadorder.scss';

import ToggleBound from '../common/togglebound';
import Form from '../common/form';

class UploadOrder extends React.Component {
    constructor(props) {
        super(props);
        this.state = { inbound: true, filename: "" }
    }

    async fileSelected(e){
        let filename = e.target.value.split("fakepath\\")[1],
            uploadFormElem = [
                {
                    name: 'File Name',
                    placeholder: 'Choose File',
                    value: filename,
                    errMsg: '',
                    required: true,
                    disabled: true,
                    valid: false,
                    field: {
                        type: "text"
                    },
                    gridClass: "col-12",
                    check: [
                        {
                            regex: /.*\.(xls)/g,
                            message: "Invalid file format"
                        }
                    ]
                }
            ];
        await this.setState({filename: filename});
        this.refs.formRef.onFormChange(filename, uploadFormElem[0])
    }

    modalFormSubmit(data) {
        console.log(data);
    }

    render() {
        let uploadFormElems = [
            {
                name: 'File Name',
                placeholder: 'Choose File',
                value: this.state.filename,
                errMsg: '',
                required: true,
                disabled: true,
                valid: false,
                field: {
                    type: "text"
                },
                gridClass: "col-12",
                check: [
                    {
                        regex: /.*\.(xls)/g,
                        message: "Invalid file format"
                    }
                ]
            }
        ];
        return (
            <React.Fragment>
                <div className="text-right">
                    <ToggleBound toggle={this.state.inbound} onClick={() => this.setState({ inbound: !this.state.inbound })} />
                </div>
                <div className="tabs-wrap">
                    <div className="tabs-header-wrap">
                        <div className="tabs-title d-none d-md-block d-lg-block active">Upload Order</div>
                        <div className="clearfix"></div>
                    </div>
                    <div className="tabs-content d-flex">
                        <Form
                            fields={uploadFormElems}
                            className="upload-form col-12 col-md-6 col-lg-6 px-2"
                            footerClassName="d-none"
                            onSubmit={obj => this.modalFormSubmit(obj)}
                            ref="formRef"
                        />
                        <React.Fragment>
                            <button className="text-uppercase btn btn-primary choose-button px-5 mt-0 ml-3" onClick={() => document.querySelector("#uploadfileinput").click()}>CHOOSE FILE</button>
                            <button className="text-uppercase btn btn-primary upload-button px-5 mt-0 ml-3" onClick={() => this.refs.formRef.onFormSubmit()}>UPLOAD</button>
                            <input type="file" id="uploadfileinput" className="d-none" onChange={e => this.fileSelected(e)} />
                        </React.Fragment>
                    </div>
                </div>
            </React.Fragment>
        );
    }
}

export default UploadOrder;