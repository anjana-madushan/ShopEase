import React from 'react'

export default function AdminTable() {
    return (
        <div className="container">
          <div className="table-wrapper">
            <table className="table custom-table">
              <thead>
                <tr>
                  <th scope="col">Username</th>
                  <th scope="col">Password</th>
                  <th scope="col">Approval Status</th>
                  <th scope="col">Approved By</th>
                  <th scope="col">Deactivated</th>
                  <th scope="col">Deactivated By</th>
                  <th scope="col">Reactivated By</th>
                </tr>
              </thead>
              <tbody>
                {tableData.map((row, index) => (
                  <tr key={index}>
                    <td>
                       {row.type}
                    </td>
                    <td>{row.period}</td>
                    <td>{row.period}</td>
                    <td>{row.period}</td>
                    <td>{row.duration}</td>
                    <td>
                      {row.status}
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
