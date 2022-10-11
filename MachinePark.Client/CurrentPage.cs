public class CurrentPage
    {
        public string CurrentPageName { get; private set; }
        public string CurrentPageDescription { get; private set; }

        public void SetCurrentPageName(string name)
        {
            if (!string.Equals(CurrentPageName, name)) 
            {
                CurrentPageName = name;
                NotifyStateChanged();
            }
        }

        public void SetCurrentPageDescription(string desc)
        {
            if (!string.Equals(CurrentPageDescription, desc)) 
            {
                CurrentPageDescription = desc;
                NotifyStateChanged();
            }
        }


        public event Action OnChange; // event raised when changed

        private void NotifyStateChanged() => OnChange?.Invoke();
    }