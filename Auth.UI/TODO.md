# Build Fix Plan - NavigationManager vs Navigation

## Root Cause
`_Imports.razor` injects the navigation service with the property name `Navigation`:
```razor
@inject NavigationManager Navigation
```
But the code-behind `.razor.cs` files reference `NavigationManager` (the type name) instead of `Navigation`. This causes:
- **CS0103**: `'NavigationManager'` does not exist in current context
- **CS0120**: object reference required for non-static `NavigationManager.NavigateTo/BaseUri`

## Solution (Option A)
Rename `NavigationManager` → `Navigation` in all affected code-behind files to match the `_Imports.razor` inject and the `MainLayout` convention.

## Steps
- [x] **Step 1**: Read all relevant files (completed)
- [x] **Step 2**: Confirm plan with user (completed - Option A chosen)
- [x] **Step 3**: Fix `LoginPage.razor.cs`
- [x] **Step 4**: Fix `VerifyDevicePage.razor.cs`
- [x] **Step 5**: Fix `SignupPage.razor.cs`
- [x] **Step 6**: Fix `PasskeySetupPage.razor.cs`
- [x] **Step 7**: Fix `ResetPasswordPage.razor.cs`
- [x] **Step 8**: Fix `Fido2Page.razor.cs`
- [x] **Step 9**: Fix `PasskeyLoginPage.razor.cs`
- [x] **Step 10**: Fix `ProfilePage.razor.cs`
- [x] **Step 11**: Build and verify (0 errors)
