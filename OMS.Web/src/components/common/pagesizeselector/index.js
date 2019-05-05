import React from 'react';
import './pagesizeselector.scss';

import { entriesPerPage } from '../../../config';

function PageSizeSelector(props) {
    let { NumberOfRecords, onChange, value } = props;
    return (
        Boolean(NumberOfRecords) &&
        <div className="table-pagesize px-2 mt-3">
            <span>Showing</span>
            <div className="form-group d-inline-block mx-2 my-0">
                <select className="form-control" value={value} onChange={e => onChange(e.target.value)}>
                    {
                        entriesPerPage.map(x =>
                            <option key={x} value={x}>{x}</option>
                        )
                    }
                </select>
            </div>
            <span>rows per page</span>
        </div>
    );
}

export default PageSizeSelector;