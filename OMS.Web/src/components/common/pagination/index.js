import React from 'react';
import './pagination.scss';
import classNames from 'classnames/bind';

function Pagination(props) {
    let { PageSize, PageNumber, NumberOfRecords, onClick } = props,
        arrayLength = Math.ceil(NumberOfRecords / PageSize),
        pageArray = [],
        rowEndCount = PageNumber*PageSize;

    while (Boolean(arrayLength)) {
        pageArray.push(arrayLength);
        arrayLength--
    }

    pageArray = pageArray.reverse();

    return (
        Boolean(NumberOfRecords) &&
        <React.Fragment>
            <div className="pagination-details">
                <p>Menampilkan {((PageNumber - 1)*PageSize) + 1} sampai {(rowEndCount > NumberOfRecords)? NumberOfRecords : rowEndCount} dari {NumberOfRecords} data</p>
            </div>
            <div className="pagination-wrap">
                <nav>
                    <ul className="pagination">
                        <li className={classNames("page-item ml-auto", { "disabled": (PageNumber === 1) })} onClick={() => ((PageNumber === 1) ? null : onClick(PageNumber - 1))}>
                            <span className="page-link">PREV</span>
                        </li>
                        {
                            pageArray.map(x =>
                                <li key={x} className={classNames("page-item", { "active": (PageNumber === x) })} onClick={() => ((PageNumber === x) ? null : onClick(x))}>
                                    <span className="page-link">{x}</span>
                                </li>
                            )
                        }
                        <li className={classNames("page-item", { "disabled": (PageNumber === pageArray.reverse()[0]) })} onClick={() => ((PageNumber === pageArray[0]) ? null : onClick(PageNumber + 1))}>
                            <span className="page-link">NEXT</span>
                        </li>
                    </ul>
                </nav>
            </div>
        </React.Fragment>
    );
}

export default Pagination;