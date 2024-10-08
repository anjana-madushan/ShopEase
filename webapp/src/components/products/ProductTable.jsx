import React from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import '../../styles/table.css'; // Custom CSS for the table design
import { FaPlane, FaUser } from 'react-icons/fa'; // FontAwesome Icons

export default function TableComponent() {
    const tableData = [
      {
        type: 'Annual leave',
        icon: <FaPlane />,
        period: 'Thu Mar 25 2021 ➔ Fri Mar 26 2021',
        duration: '2 days',
        status: 'Pending',
        statusClass: 'badge-pending'
      },
      {
        type: 'Remote',
        icon: <FaUser />,
        period: 'Tue Feb 17 2020 ➔ Thu Feb 19 2020',
        duration: '3 days',
        status: 'Pending',
        statusClass: 'badge-pending'
      },
      {
        type: 'Remote',
        icon: <FaUser />,
        period: 'Mon Dec 02 2020 ➔ Mon Dec 02 2020',
        duration: '1 day',
        status: 'Approved',
        statusClass: 'badge-approved'
      },
      {
        type: 'Remote',
        icon: <FaUser />,
        period: 'Mon Nov 25 2020 ➔ Fri Nov 29 2020',
        duration: '4 days',
        status: 'Approved',
        statusClass: 'badge-approved'
      },
      {
        type: 'Annual leave',
        icon: <FaPlane />,
        period: 'Thu Sep 14 2020 ➔ Fri Sep 15 2020',
        duration: '1 day',
        status: 'Declined',
        statusClass: 'badge-declined'
      }
    ];
  
    return (
      <div className="container">
        <div className="table-wrapper">
          <table className="table custom-table">
            <thead>
              <tr>
                <th scope="col">Type</th>
                <th scope="col">Request Period</th>
                <th scope="col">Duration</th>
                <th scope="col">Status</th>
                <th scope="col">Actions</th>
              </tr>
            </thead>
            <tbody>
              {tableData.map((row, index) => (
                <tr key={index}>
                  <td>
                    <span className="icon">{row.icon}</span> {row.type}
                  </td>
                  <td>{row.period}</td>
                  <td>{row.duration}</td>
                  <td>
                    <span className={`badge ${row.statusClass}`}>{row.status}</span>
                  </td>
                  <td>
                    <a href="#" className="details-link">
                      Details
                    </a>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    );
  }
