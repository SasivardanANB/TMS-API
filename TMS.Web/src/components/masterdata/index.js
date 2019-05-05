import React from 'react';
import './masterdata.scss';
import CustomBox from '../common/custombox';

const renderBox = [
    {
        name: 'Expeditor',
        circleBgColor: 'bg-primary',
        logo: 'fas fa-truck-moving',
        URL: '/masterdata/maintainexpeditor'
    },
    {
        name: 'Vehice',
        circleBgColor: 'bg-success',
        logo: 'fas fa-truck',
        URL: '/masterdata/maintainvehicle'
    },
    {
        name: 'Admin',
        circleBgColor: 'bg-danger',
        logo: 'fas fa-user-tie',
        URL: '/masterdata/maintainadmintransporter'
    },
    {
        name: 'Drivers',
        circleBgColor: 'bg-info',
        logo: 'fas fa-wheelchair',
        URL: '/masterdata/maintaindrivertransporter'
    },
    {
        name: 'Pools',
        circleBgColor: 'bg-warning',
        logo: 'fas fa-map-marker-alt',
        URL: '/masterdata/maintainpools'
    },
    
]

class Masterdata extends React.Component {
    render() {
        return (
            <div className="row shadow-sm bg-light m-0 p-3">

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

export default Masterdata;