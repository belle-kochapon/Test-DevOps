#!/bin/bash

. ./export-local.bash

TOKEN=eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI4cEd1bk9aMC1vLUdCV3FTUTM1U05XcUNXT1BMOG1fN200V2lhR2xwYUxVIn0.eyJleHAiOjE3MDIzODQ5MzMsImlhdCI6MTcwMjM4NDYzMywianRpIjoiYjkwMzIxNTgtYzRjZi00NzI2LWFkMTAtNTE2ZGM1ODZhM2VhIiwiaXNzIjoiaHR0cHM6Ly9rZXljbG9hay5wcm9taWQucHJvbS5jby50aC9hdXRoL3JlYWxtcy9wcm9taWQiLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiYjcwYjVjZWUtMGIzMy00ZDBiLWI3OWUtZDFmNWQzMDI2NGJiIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoibHByLWFwaSIsInNlc3Npb25fc3RhdGUiOiI1NTgyM2NlOS1jZjhmLTQwMDEtOGJmZS1kZWFlNmMzYTZmMzkiLCJhY3IiOiIxIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iLCJkZWZhdWx0LXJvbGVzLXByb21pZCJdfSwicmVzb3VyY2VfYWNjZXNzIjp7ImFjY291bnQiOnsicm9sZXMiOlsibWFuYWdlLWFjY291bnQiLCJtYW5hZ2UtYWNjb3VudC1saW5rcyIsInZpZXctcHJvZmlsZSJdfX0sInNjb3BlIjoiZW1haWwgcHJvZmlsZSIsInNpZCI6IjU1ODIzY2U5LWNmOGYtNDAwMS04YmZlLWRlYWU2YzNhNmYzOSIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwibmFtZSI6IlN1cHJlZXlhIE1vbnNhciIsInByZWZlcnJlZF91c2VybmFtZSI6InN1cHJlZXlhIiwiZ2l2ZW5fbmFtZSI6IlN1cHJlZXlhIiwiZmFtaWx5X25hbWUiOiJNb25zYXIiLCJlbWFpbCI6ImljeUBnbWFpbC5jb20ifQ.oK7R1CoPVWdb-wCcdKso_P7JCYxzUSLrqZ4UdkgW48qxtkywA6uuETAXDCg1xeUGynwxj5DEZ9lSGk3MhK1_qqQPk4bI8ntjXpwaNUSWH0cScY0dMI8Uyo7V5QDsOHYNZzYaQqX8oosmJMZJBV-DLIyyWYLjkPM00BZrmzVcxqbggbEoIaNCoHdTYgMTClE7Ik4VdYLZic-6zt-yDNMwMZMjmQpP7sLVwzF75kD-RCMzw72LI2ZKdVamu2FfhU2yG9s_eXIrlBO7eWmNsff0FwvqVkDpr-RrK9BSNJjlIPHTInce3hv3Vxc7TAuyu0Re2nWBHZKAJyPm-b03l_MVAg
TOKEN_BASE64=$(echo -n ${TOKEN} | base64 -w0 )

curl -X GET ${ENDPOINT_GET_USER_ALLOWED_ORG}/supreeya -u ${AUTH_USER}:${AUTH_PASSWORD_GLB}

#-H "Authorization: Bearer ${TOKEN_BASE64}"
# -u ${AUTH_USER}:${AUTH_PASSWORD_GLB}