import React from 'react';
import './custombox.scss';
import classNames from 'classnames/bind';

class CustomBox extends React.Component{
    constructor(props) {
        super(props);
        this.state={

        };
    }

    render(){
        console.log(".........", this.props);
        return (
            <div className="row custom-box border rounded-lg px-2 py-3 m-0 my-2" onClick={() => this.props.onClick(this.props.URL)}>
                <div className="col-5 col-md-4 col-lg-4">
                    <div className={classNames("circle", this.props.circleBgColor)}>
                        <i className={classNames("text-light", "fa-2x", this.props.logo)}></i>
                    </div>
                </div>
                <div className="col-7 col-md-8 col-lg-8 p-0">
                    <div className="">
                        <h5 className="m-0 txt-clr">{this.props.name}</h5>
                    </div>
                </div>
            </div>
        );
    }
    
}

export default CustomBox;
