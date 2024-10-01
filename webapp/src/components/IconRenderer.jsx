import React from 'react';
import { FaHome, FaBell, FaChartBar, FaShoppingCart, FaUsers, FaCog } from 'react-icons/fa';

const IconRenderer = ({ iconKey }) => {
  switch (iconKey) {
    case 'FaHome':
      return <FaHome />;
    case 'FaBell':
      return <FaBell />;
    case 'FaChartBar':
      return <FaChartBar />;
    case 'FaShoppingCart':
      return <FaShoppingCart />;
    case 'FaUsers':
      return <FaUsers />;
    case 'FaCog':
      return <FaCog />;
    default:
      return null;
  }
};

export default IconRenderer;
