import React, { useState, useEffect } from "react";
import { getAllUsers, getApprovedCus, getUnapprovedCus ,getUserById} from "../../api/services/authService"; 
import { useSelector } from "react-redux";
import { ButtonGroup, Button, Badge } from "react-bootstrap";

export default function CustomerList() {
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [activeTab, setActiveTab] = useState("all"); 
  const [usernames, setUsernames] = useState({});

  // Get the token from the Redux store
  const token = useSelector((state) => state.auth.loggedUser.token);

  const fetchCustomers = async (type) => {
    setLoading(true);
    setError(null);
    let fetchApi;

    // Choose the appropriate API based on the button clicked
    if (type === "all") {
      fetchApi = () => getAllUsers(token, "customer");
    } else if (type === "approved") {
      fetchApi = () => getApprovedCus(token);
    } else if (type === "unapproved") {
      fetchApi = () => getUnapprovedCus(token);
    }

    // Fetch data from the selected API
    try {
      const response = await fetchApi();
      setCustomers(response); 
      setLoading(false);

      const newUsernames = {};
      for (const customer of response) {
        if (customer.approvedBy) {
          const userResponse = await getUserById(customer.approvedBy, token);
          newUsernames[customer.approvedBy] = userResponse.username; 
        }
      }
      setUsernames(newUsernames);
    } catch (error) {
      setError("Failed to fetch customer data");
      setLoading(false);
    }
  };

  useEffect(() => {
    if (token) {
      fetchCustomers("all");
    }
  }, [token]);
  
  const handleTabChange = (type) => {
    setActiveTab(type);
    fetchCustomers(type);
  };

  const renderApprovalStatus = (status) => {
    return status === true ? (
      <Badge bg="success">Approved</Badge>
    ) : (
      <Badge bg="secondary">Pending</Badge>
    );
  };
  const renderActiveStatus = (status) => {
    return status === false ? (
      <Badge bg="success">Active</Badge>
    ) : (
      <Badge bg="danger">Deactive</Badge>
    );
  };

  if (loading) {
    return <p>Loading...</p>;
  }

  if (error) {
    return <p>{error}</p>;
  }

  return (
    <div className="container">
      <ButtonGroup className="mb-3 d-flex justify-content-between">
        <Button
          variant="primary"
          className="mx-2 flex-fill"
          onClick={() => handleTabChange("all")}
          active={activeTab === "all"}
        >
          All Customers
        </Button>
        <Button
          variant="primary"
          className="mx-2 flex-fill"
          onClick={() => handleTabChange("approved")}
          active={activeTab === "approved"}
        >
          Approved
        </Button>
        <Button
          variant="primary"
          className="mx-2 flex-fill"
          onClick={() => handleTabChange("unapproved")}
          active={activeTab === "unapproved"}
        >
          Unapproved
        </Button>
      </ButtonGroup>

      <div className="table-wrapper">
        <table className="table custom-table">
          <thead>
            <tr>
              <th scope="col">Username</th>
              <th scope="col">Email</th>
              <th scope="col">Approval Status</th>
              <th scope="col">Approved By</th>
              <th scope="col">Activated Status</th>
              <th scope="col">Deactivated By</th>
              <th scope="col">Reactivated By</th>
            </tr>
          </thead>
          <tbody>
            {customers.length > 0 ? (
              customers.map((customer) => (
                <tr key={customer.id}>
                  <td>{customer.username}</td>
                  <td>{customer.email}</td>
                  <td>{renderApprovalStatus(customer.approvalStatus)}</td>
                  <td>{usernames[customer.approvedBy] || "N/A"}</td>
                  <td>{renderActiveStatus(customer.deactivated)}</td>
                  <td>{usernames[customer.deactivatedBy] || "N/A"}</td>
                  <td>{usernames[customer.reactivatedBy] || "N/A"}</td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="7">No customers found</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
