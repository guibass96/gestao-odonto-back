using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.Infrastructure.Identity
{
    public enum Roles
    {
        SUPER_ADMIN,     // Full system access (SaaS platform owner)
        TENANT_OWNER,    // Owner of the tenant (e.g., clinic owner)
        OWNER,           // Could be a general owner type
        DENTIST_OWNER,   // Dentist who also owns the practice
        DENTIST,         // Hired or associated dentist
        SECRETARY,       // Similar to receptionist
        ASSISTANT,       // Assistant to dentist
        MANAGER,         // Branch or general clinic manager
        USER             // Basic user (e.g., patient)
    }
}
