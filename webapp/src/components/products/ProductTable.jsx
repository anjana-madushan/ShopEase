import React from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import '../../styles/table.css'; // Custom CSS for the table design
import { FaPlane, FaUser } from 'react-icons/fa'; // FontAwesome Icons

export default function ProductTable() {
    
  
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
