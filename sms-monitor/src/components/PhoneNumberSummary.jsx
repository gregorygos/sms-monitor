import React, { useState, useEffect } from 'react';
import { getFilteredMessages, getMessagesPerSecond } from '../services/messageService';
import dummyData from '../data/dummyData.js';

export default function PhoneNumberSummary ({ useDummyData = false }) {
  const refreshInterval = 10000;
  const [messages, setMessages] = useState([]);
  const [messagesPerSecond, setMessagesPerSecond] = useState(0);
  const [filters, setFilters] = useState({
    phoneNumber: '',
    startDate: '',
    endDate: ''
  });

  useEffect(() => {
    if (!useDummyData) {
      // Fetch messages per second periodically
      const interval = setInterval(() => {
        getMessagesPerSecond().then(response => {
          setMessagesPerSecond(response.data);
        }).catch(error => {
          console.error('Error fetching messages per second:', error);
        });
      }, refreshInterval);

      return () => clearInterval(interval);
    }
  }, [useDummyData]);

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilters({
      ...filters,
      [name]: value
    });
  };

  const applyFilters = () => {
    if (useDummyData) {
      const filteredMessages = dummyData.filter(message => {
        const isPhoneNumberMatch = filters.phoneNumber ? message.phoneNumber.includes(filters.phoneNumber) : true;
        const isStartDateMatch = filters.startDate ? new Date(message.messageTimestamps[0]) >= new Date(filters.startDate) : true;
        const isEndDateMatch = filters.endDate ? new Date(message.messageTimestamps[message.messageTimestamps.length - 1]) <= new Date(filters.endDate) : true;
        return isPhoneNumberMatch && isStartDateMatch && isEndDateMatch;
      });

      setMessages(filteredMessages);
    } else {
      getFilteredMessages(filters).then(response => {
        setMessages(response.data);
      }).catch(error => {
        console.error('Error fetching filtered messages:', error);
      });
    }
  };

  return (
    <div>
      <h2>Messages Per Number</h2>
      <div>
        <input
          type="text"
          name="phoneNumber"
          placeholder="Phone Number"
          value={filters.phoneNumber}
          onChange={handleFilterChange}
        />
        <input
          type="date"
          name="startDate"
          value={filters.startDate}
          onChange={handleFilterChange}
        />
        <input
          type="date"
          name="endDate"
          value={filters.endDate}
          onChange={handleFilterChange}
        />
        <button onClick={applyFilters}>Apply Filters</button>
      </div>
      <div>
        <h3>Messages Per Second: {useDummyData ? "0 (Using Dummy Data)" : messagesPerSecond}</h3>
      </div>
      <ul>
        {messages.map((message, index) => (
          <li key={index}>
            <strong>Phone Number:</strong> {message.phoneNumber} <br />
            <strong>Account ID:</strong> {message.accountId} <br />
            <strong>Message Timestamps:</strong>
            <ul>
              {message.messageTimestamps.map((timestamp, i) => (
                <li key={i}>{timestamp}</li>
              ))}
            </ul>
          </li>
        ))}
      </ul>
    </div>
  );
};
