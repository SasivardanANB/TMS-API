import React from 'react';
import './report.scss';
import CustomBox from '../common/custombox';

const renderBox = [
    {
        name: 'Report Order Per Hari',
        circleBgColor: 'bg-primary',
        logo: 'fas fa-file-alt',
        URL: ''
    },
    {
        name: 'Armada Muat Per Hari',
        circleBgColor: 'bg-success',
        logo: 'fas fa-truck',
        URL: ''
    },
    {
        name: 'Armada Bongkar Per Hari',
        circleBgColor: 'bg-danger',
        logo: 'fas fa-truck',
        URL: ''
    },
    {
        name: 'Board Admin',
        circleBgColor: 'bg-info',
        logo: 'fas fa-user',
        URL: ''
    },
    {
        name: 'Order Progress',
        circleBgColor: 'bg-warning',
        logo: 'fas fa-box',
        URL: ''
    },
    {
        name: 'Invoice',
        circleBgColor: 'brown-bgclr',
        logo: 'fas fa-file-invoice-dollar',
        URL: ''
    },
    {
        name: 'GR vs Order',
        circleBgColor: 'blue-bgclr',
        logo: 'fas fa-box',
        URL: ''
    },
    {
        name: 'GI vs Order',
        circleBgColor: 'purple-bgclr',
        logo: 'fas fa-box',
        URL: ''
    },
    
]

class Report extends React.Component {
    render() {
        console.log(this.props);
        return (
            <div className="row shadow-sm bg-light m-0 p-3 report">

                {
                    renderBox.map((x) =>
                        <div key={x.name} className="col-12 col-md-4 col-lg-4">
                            <CustomBox {...x} onClick={x => this.props.history.push(x)} />
                        </div>
                    )
                }

            </div>

        );
    }
}

export default Report;