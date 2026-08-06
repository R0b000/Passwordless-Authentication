# CSS Fix Plan - TODO

## Root Cause
`.razor.css` files use CSS isolation - styles are scoped only to that component. Login page styles in `LoginLayout.razor.css` don't apply to `Login_Page.razor` elements.

## Steps

- [x] **Step 1**: Read all relevant files (completed)
- [x] **Step 2**: Get plan approval (completed)
- [x] **Step 3**: Add global CSS variables to `wwwroot/css/app.css`
- [x] **Step 4**: Move login page styles from `LoginLayout.razor.css` to `Login_Page.razor.css`
- [x] **Step 5**: Clean up `LoginLayout.razor.css` (keep minimal)
- [x] **Step 6**: Add `html, body { height: 100% }` to `app.css`
- [x] **Step 7**: Build and verify
