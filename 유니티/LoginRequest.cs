using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginRequest
{

    public string userEmail;
    public string userPassword;

    public LoginRequest(string userEmail, string userPassword)
    {
        this.userEmail = userEmail;
        this.userPassword = userPassword;
    }

    public string getUserEmail()
    {
        return userEmail;
    } 
    public string getUserPassword()
    {
        return userPassword;
    }
}

public class RegistRequest
{
    public string userEmail;
    public string userPassword;
    public string userNickname;

    public RegistRequest(string userEmail, string userPassword, string userNickname)
    {
        this.userEmail = userEmail;
        this.userPassword = userPassword;
        this.userNickname = userNickname;
    }
}

public class ReissueRequest
{
    public string refreshToken;
    public string userEmail;

    public ReissueRequest(string refreshToken, string userEmail)
    {
        this.refreshToken = refreshToken;
        this.userEmail = userEmail;
    }
}