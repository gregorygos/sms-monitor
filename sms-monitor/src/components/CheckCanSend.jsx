import React, { useState } from 'react';
import { checkCanSendMessage } from '../services/messageService';

export default function CheckCanSend() {
  const [phoneNumber, setPhoneNumber] = useState('');
  const [accountId, setAccountId] = useState('');
  const [canSend, setCanSend] = useState(null);

  function handleCheckCanSend() {
    checkCanSendMessage(phoneNumber, accountId).then(response => {
      setCanSend(response.data.canSend);
    }).catch(error => {
      console.error('Error checking if message can be sents:', error);
      setCanSend(null);
    });
  };

  return (
    <div>
      <h2>Check Can Send Message</h2>
      <input
        type="text"
        placeholder="Phone Number"
        value={phoneNumber}
        onChange={(e) => setPhoneNumber(e.target.value)}
      />
      <input
        type="text"
        placeholder="Account ID"
        value={accountId}
        onChange={(e) => setAccountId(e.target.value)}
      />
      <button onClick={handleCheckCanSend}>Check Can Send</button>
      {canSend !== null && (
        <div>
          <strong>Can Send:</strong> {canSend ? "Yes" : "No"}
        </div>
      )}
    </div>
  );
};
