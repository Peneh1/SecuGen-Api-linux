#ifndef SEARCHSEARCH_EXCEPTION_EADER_INCLUDE_
#define SEARCHSEARCH_EXCEPTION_EADER_INCLUDE_

#include <exception>
#include <string>
#include <sstream>
#include "SecuSearchErr.h"

class SecuSearchException : public std::exception
{
public:
    SecuSearchException() = delete;

    SecuSearchException(SSE_ERROR code)
        : errorCode(code)
        , message(makeDefaultMessage(code))
    {
    }

    SecuSearchException(SSE_ERROR code, const std::string& message)
        : errorCode(code)
        , message(std::move(message))
    {
    }

    const char* what() const noexcept override
    {
        return message.c_str();
    }

    SSE_ERROR code() const noexcept
    {
        return errorCode;
    }

protected:

    static std::string makeDefaultMessage(SSE_ERROR code)
    {
        std::stringstream s;
        s << "SecuSearch Exception Code: " << code;
        return s.str();
    }

    const std::string message;
    const SSE_ERROR errorCode;
};

#endif SEARCHSEARCH_EXCEPTION_EADER_INCLUDE_
