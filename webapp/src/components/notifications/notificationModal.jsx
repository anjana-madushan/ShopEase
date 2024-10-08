/* eslint-disable react/prop-types */
/* eslint-disable react/no-unknown-property */
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { getNotificationsBasedOnVendor } from "../../api/services/notifications";
import Modal from 'react-bootstrap/Modal';

export default function NotificationModal({ show, handleClose }) {
  const [notifications, setNotificationData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Get the token from the Redux store
  const token = useSelector((state) => state.auth.loggedUser.token);

  useEffect(() => {
    const fetchNotifications = async () => {
      if (!token) {
        setError("No token found. Please log in.");
        setLoading(false);
        return;
      }

      try {
        const response = await getNotificationsBasedOnVendor(token);
        console.log("API Response:", response);

        if (Array.isArray(response.data)) {
          setNotificationData(response.data);
        } else if (typeof response.data === "string") {
          setNotificationData([]);
          setError(response.data);
        }
      } catch (error) {
        console.error("Error fetching notifications:", error.response || error);
        setError("Failed to fetch notifications");
      } finally {
        setLoading(false);
      }
    };

    fetchNotifications();
  }, [token]);

  // Render loading message while data is being fetched
  if (loading) {
    return <p>Loading...</p>;
  }

  // Render error message if fetching failed
  if (error) {
    return <p>{error}</p>;
  }

  return (
    <Modal show={show} onHide={handleClose}>
      <Modal.Header closeButton>
        <Modal.Title>Notifications</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {notifications.length > 0 ? (
          notifications.map((notification, index) => (
            <div key={index} style={{ marginBottom: '10px' }}>
              <p>{notification.message}</p>
              <small>{new Date(notification.date).toLocaleString()}</small>
              {index < notifications.length - 1 && <hr />}
            </div>
          ))
        ) : (
          <p>No notifications found.</p>
        )}
      </Modal.Body>
      <Modal.Footer>
        <button variant="secondary" onClick={handleClose}>
          Close
        </button>
      </Modal.Footer>
    </Modal>
  );
}
