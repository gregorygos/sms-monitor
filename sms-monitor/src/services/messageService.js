import axios from 'axios';

const apiUrl = 'http://localhost:4000/Message';

export const getAllMessages = () => {
  return axios.get(`${apiUrl}/all-messages`);
};

export const getMessagesPerSecond = () => {
  return axios.get(`${apiUrl}/messages-per-second`);
};

export const getFilteredMessages = (filters) => {
  const params = new URLSearchParams(filters).toString();
  return axios.get(`${apiUrl}/filtered-messages?${params}`);
};

export const getMessagesByAccount = (filters) => {
  const params = new URLSearchParams(filters).toString();
  return axios.get(`${apiUrl}/messages-by-account?${params}`);
};

export const checkCanSendMessage = (phoneNumber, accountId) => {
    return axios.post(`${apiUrl}/can-send`, {
      phoneNumber: phoneNumber,
      accountId: accountId
    });
  };
  
